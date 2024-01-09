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
    public class PostsController : ControllerBase
    {
        private readonly VerboseContext _context;

        public PostsController(VerboseContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> SubmitPost([FromBody] SubmitPostBody body)
        {
            Post post = null;


            PublicProfile pp = _context.PublicProfiles
                .Where(x => x.PublicProfileID.Equals(int.Parse(body.posterID)))
                .Include(x => x.Posts)
                .FirstOrDefault();

            Episode episode = null;
            if (body.podchaserID != null)
            {
                episode = _context.Episodes
                .Where(x => x.EpisodeID.Equals(int.Parse(body.podchaserID)))
                .FirstOrDefault();
            }
            string imageURL = "";
            if (body.imageURL != null)
            {
                imageURL = body.imageURL;
            }

            if (pp != null)
            {
                post = new Post
                {
                    Title = body.title,
                    Description = body.description,
                    Date = DateTime.Parse(body.date),
                    ImageURL = imageURL,
                    PublicProfileId = int.Parse(body.posterID),
                    Episode = episode,
                    Comments = new List<Comment>(),
                    LikedBy = new HashSet<LikedBy>(),
                    Likes = 0,
                };

                pp.Posts.Insert(0, post);
                _context.Posts.Add(post);
                _context.SaveChanges();
                return Ok();
            }

            return BadRequest();

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LikeOrUnlike([FromBody] LikeOrUnlikeBody body)
        {


            int pID = int.Parse(body.postID);
            bool likeOrUnlike = bool.Parse(body.likeOrUnlike);
            int publicProfileID = int.Parse(body.publicProfile);

            var post = (_context.Posts
                .Where(x => x.PostID.Equals(pID))
                .Include(x => x.LikedBy)
                .FirstOrDefault());

            // Like = true, unlike = false
            if (post != null)
            {
                post.Likes += likeOrUnlike ? 1 : -1;


                LikedBy likedBy = new LikedBy
                {
                    PostID = pID,
                    PublicProfileID = publicProfileID,
                };
                if (likeOrUnlike)
                {

                    post.LikedBy.Add(likedBy);
                }
                else
                {
                    post.LikedBy.Remove(likedBy);
                }

                _context.Posts.Update(post);

                _context.SaveChanges();
                return Ok();
            }

            return BadRequest();

        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await _context.Posts.ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            if (id != post.PostID)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.PostID }, post);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostID == id);
        }
    }

    public class SubmitPostBody
    {
        public string posterID { get; set; }
        public string? podchaserID { get; set; }
        public string imageURL { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string date { get; set; }
    }

    public class LikeOrUnlikeBody
    {
        public string postID { get; set; }
        public string likeOrUnlike { get; set; }
        public string publicProfile { get; set; }
    }
}
