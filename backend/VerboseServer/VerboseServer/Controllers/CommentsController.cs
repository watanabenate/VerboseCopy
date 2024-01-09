#nullable disable
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

namespace VerboseServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly VerboseContext _context;

        public CommentsController(VerboseContext context)
        {
            _context = context;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComment()
        {
            return await _context.Comments.ToListAsync();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, Comment comment)
        {
            if (id != comment.CommentID)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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

        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.CommentID }, comment);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> SubmitComment([FromBody] SubmitCommentBody body)
        {
            try
            {

                Comment comment = null;
                int episodeID = body.podchaserID != null ? int.Parse(body.podchaserID) : -1;
                int postID = body.post != null ? int.Parse(body.post) : -1;
                int commenterID = int.Parse(body.commenter);

                long timestamp = -1;
                if(body.timestamp != null)
                {
                    timestamp = body.timestamp != "" ? long.Parse(body.timestamp) : -1;
                }

                PublicProfile publicProfile = _context.PublicProfiles
                    .Where(x => x.PublicProfileID.Equals(commenterID))
                    .Include(x => x.Posts)
                    .ThenInclude(x => x.Comments)
                    .FirstOrDefault();

                if (publicProfile == null) { return BadRequest(); }

                comment = new Comment
                {
                    Text = body.text,
                    Date = DateTime.Parse(body.date),
                    Timestamp = timestamp,
                    Likes = 0,
                    Replies = new List<Comment>(),
                    CommentBy = new CommentBy
                    {
                        PublicProfileID = commenterID
                    },

                };

                if (episodeID != -1)
                {
                    Episode episode = await _context.Episodes
                    .Where(x => x.EpisodeID.Equals(episodeID))
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync();

                    if (episode != null)
                    {
                        episode.Comments.Add(comment); // TODO: Do a sort of these comments eventually
                        await _context.Comments.AddAsync(comment);
                    }
                    episode.Comments.Add(comment);
                    episode.Comments.Sort(new CommentTimestampComparer());
                    _context.Comments.Add(comment);
                    _context.Episodes.Update(episode);
                }
                else
                {
                    Post post = await _context.Posts
                    .Where(x => x.PostID.Equals(postID))
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync();

                    if (post != null)
                    {
                        post.Comments.Insert(0, comment);
                        _context.Comments.Add(comment);
                        comment.CommentBy.CommentID = comment.CommentID;
                        _context.Posts.Update(post);
                    }
                }

                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest();
            }

        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentID == id);
        }

        private class CommentTimestampComparer : Comparer<Comment>
        {
            public override int Compare(Comment x, Comment y)
            {
                return x.Timestamp.CompareTo(y.Timestamp);
            }
        }
    }

    public class SubmitCommentBody
    {
        public string commenter { get; set; }
        public string? timestamp { get; set; }
        public string text { get; set; }
        public string date { get; set; }
        public string? podchaserID { get; set; }
        public string? post { get; set; }
    }
}
