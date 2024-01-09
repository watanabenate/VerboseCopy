using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VerboseServer.Data;
using VerboseServer.Models;
using VerboseServer.Models.Responses;

namespace VerboseServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodesController : ControllerBase
    {
        private readonly VerboseContext _context;

        public EpisodesController(VerboseContext context)
        {
            _context = context;
        }

        // GET: api/Episodes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Episode>>> GetEpisodes()
        {
            return await _context.Episodes.ToListAsync();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListenedTo([FromBody] ListenedToBody body)
        {
            try
            {
                int profileID = int.Parse(body.ProfileID);
                int episodeID = int.Parse(body.EpisodeID);
                long timestamp = long.Parse(body.Timestamp);

                ListenedTo epi = new ListenedTo
                {
                    ProfileID = profileID,
                    EpisodeID = episodeID,
                    Timestamp = timestamp,
                    DateListened = DateTime.Now,
                };

                var profile = await _context.Profiles
                    .Include(p => p.RecentlyListenedTo)
                    .Where(p => p.ProfileID == profileID)
                    .FirstOrDefaultAsync();

                foreach (ListenedTo l in profile.RecentlyListenedTo)
                {
                    if (l.EpisodeID == episodeID)
                    {
                        l.DateListened = DateTime.Now;
                        l.Timestamp = timestamp;
                        _context.SaveChanges();
                        return Ok();
                    }
                }
                profile.RecentlyListenedTo.Add(epi);

                if (profile.RecentlyListenedTo.Count > 5)
                {
                    profile.RecentlyListenedTo.RemoveAt(0);
                }

                _context.ListenedTo.Add(epi);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<ListenedToApp>> Listen([FromBody] ListenBody body)
        {
            try
            {
                int epID = int.Parse(body.EpisodeID);
                int profID = int.Parse(body.ProfileID);

                var listenedTo = await _context.ListenedTo
                                    .Where(p => p.ProfileID == profID)
                                    .Where(p => p.EpisodeID == epID)
                                    .FirstOrDefaultAsync();

                var episode = await _context.Episodes
                                    .Include(e => e.Comments).ThenInclude(e => e.CommentBy)
                                    .Include(e => e.LikedBy)
                                    .Where(e => e.EpisodeID == epID)
                                    .FirstOrDefaultAsync();

                if(episode == null)
                {
                    return BadRequest();
                }

                ListenedToApp result = null;

                EpisodeApp episodeApp = null;
                List<CommentApp> episodeAppComments = new List<CommentApp>();
                foreach (Comment comment in episode.Comments)
                {
                    CommentApp c = new CommentApp(comment, await _context.PublicProfiles.FirstOrDefaultAsync
                        (x => x.PublicProfileID.Equals(comment.CommentBy.PublicProfileID)));

                    episodeAppComments.Add(c);
                }
                episodeApp = new EpisodeApp(episode, episodeAppComments);

                // if the user has not already listened to the episode
                if (listenedTo != null)
                {
                    result = new ListenedToApp(episodeApp, listenedTo.Timestamp);
                }
                    
                else
                {
                    result = new ListenedToApp(episodeApp, 0);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LikeOrUnlike([FromBody] LikeOrUnlikePostBody body)
        {
            int eID = int.Parse(body.EpisodeID);
            bool likeOrUnlike = bool.Parse(body.LikeOrUnlike);
            int publicProfileID = int.Parse(body.PublicProfile);

            var episode = (await _context.Episodes
                .Where(x => x.EpisodeID.Equals(eID))
                .FirstOrDefaultAsync());

            // Like = true, unlike = false
            if (episode != null)
            {
                episode.LikeCount += likeOrUnlike ? 1 : -1;

                if (likeOrUnlike) { episode.LikedBy.Add(new LikedByEpisode(publicProfileID, eID)); }
                else { episode.LikedBy.Remove(new LikedByEpisode(publicProfileID, eID)); }

                _context.Episodes.Update(episode);
                _context.SaveChanges();
                return Ok();
            }
            else
                return BadRequest();

        }

        // GET: api/Episodes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Episode>> GetEpisode(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);

            if (episode == null)
            {
                return NotFound();
            }

            return episode;
        }

        // PUT: api/Episodes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEpisode(int id, Episode episode)
        {
            if (id != episode.EpisodeID)
            {
                return BadRequest();
            }

            _context.Entry(episode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EpisodeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Episodes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Episode>> PostEpisode(Episode episode)
        {
            _context.Episodes.Add(episode);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEpisode", new { id = episode.EpisodeID }, episode);
        }

        // DELETE: api/Episodes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEpisode(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null)
            {
                return NotFound();
            }

            _context.Episodes.Remove(episode);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EpisodeExists(int id)
        {
            return _context.Episodes.Any(e => e.EpisodeID == id);
        }
    }

    public class ListenBody
    {
        public string EpisodeID { get; set; }
        public string ProfileID { get; set; }
    }

    public class LikeOrUnlikePostBody
    {
        public string EpisodeID { get; set; }
        public string LikeOrUnlike { get; set; }
        public string PublicProfile { get; set; }
    }

    public class ListenedToBody
    {
        public string ProfileID { get; set; }
        public string EpisodeID { get; set; }
        public string Timestamp { get; set; }
    }
}
