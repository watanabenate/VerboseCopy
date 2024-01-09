using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerboseServer.Data;
using VerboseServer.Models;

namespace VerboseServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PodcastsController : ControllerBase
    {
        private readonly VerboseContext _context;

        public PodcastsController(VerboseContext context)
        {
            _context = context;
        }

        // GET: api/Podcasts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VerboseServer.Models.Podcast>>> GetPodcasts()
        {
            return await _context.Podcasts.ToListAsync();
        }

        // GET: api/Podcasts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VerboseServer.Models.Podcast>> GetPodcast(int id)
        {
            var podcast = await _context.Podcasts
                .Include(x => x.Episodes)
                .Where(x => x.PodcastID == id)
                .FirstOrDefaultAsync();

            if (podcast == null)
            {
                return NotFound();
            }

            return podcast;
        }

        // PUT: api/Podcasts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPodcast(int id, VerboseServer.Models.Podcast podcast)
        {
            if (id != podcast.PodcastID)
            {
                return BadRequest();
            }

            _context.Entry(podcast).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PodcastExists(id))
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

        // POST: api/Podcasts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Podcast>> PostPodcast(VerboseServer.Models.Podcast podcast)
        {
            _context.Podcasts.Add(podcast);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPodcast", new { id = podcast.PodcastID }, podcast);
        }

        // DELETE: api/Podcasts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePodcast(int id)
        {
            var podcast = await _context.Podcasts.FindAsync(id);
            if (podcast == null)
            {
                return NotFound();
            }

            _context.Podcasts.Remove(podcast);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PodcastExists(int id)
        {
            return _context.Podcasts.Any(e => e.PodcastID == id);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeBody body)
        {
            int podcastID = int.Parse(body.podcastID);
            int publicProfileID = int.Parse(body.publicProfileID);

            try
            {
                var podcast = await _context.Podcasts
                    .Where(x => x.PodcastID == podcastID)
                    .FirstOrDefaultAsync();

                var publicProfile = await _context.PublicProfiles.FindAsync(publicProfileID);

                publicProfile.Subscribed.Add(podcast);

                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetPodcastFromEpisode(int podcastId)
        {
            var podcast = await _context.Podcasts
                .Include(x => x.Episodes)
                .Where(x => x.PodcastID == podcastId)
                .FirstOrDefaultAsync();

            if (podcast == null)
            {
                return NotFound();
            }

            return Ok(podcast);
        }
    }

    

    public class SubscribeBody { 
        public string podcastID { get; set; }
        public string publicProfileID { get; set; }
    }

    public class PodcastIdBody
    {
        public string podcastID { get; set; }
    }

}
