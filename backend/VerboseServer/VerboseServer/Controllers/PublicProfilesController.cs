#nullable disable
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
    public class PublicProfilesController : ControllerBase
    {
        private readonly VerboseContext _context;

        public PublicProfilesController(VerboseContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> FollowOrUnfollow([FromBody] FollowOrUnfollowBody body)
        {
            try
            {
                int followerID = int.Parse(body.followerID);
                int followeeID = int.Parse(body.followeeID);
                bool isFound = false;

                var following = (await _context.PublicProfiles
                                                .Include(x => x.Following)
                                                .Where(x => x.PublicProfileID == followerID)
                                                .FirstOrDefaultAsync()).Following;

                foreach (FollowedBy f in following)
                {
                    if (followeeID == f.FolloweeID)
                    {
                        following.Remove(f);
                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                    following.Add(new FollowedBy
                    {
                        FolloweeID = followeeID,
                        FollowerID = followerID
                    });

                _context.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateProfilePicture([FromBody] UpdateProfilePictureBody body)
        {
            try
            {
                int profileID = int.Parse(body.publicProfileID);
                string link = body.newPhotoLink;

                var prof = (await _context.PublicProfiles
                                    .Where(p => p.PublicProfileID == profileID)
                                    .FirstOrDefaultAsync());
                prof.PictureLink = link;
                _context.SaveChanges(); 
                return Ok(); 

            }
            catch(Exception ex)
            {
                return BadRequest(); 
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetOtherUser([FromBody] GetOtherUserBody body)
        {
            try
            {
                string username = body.Username;

                PublicProfile prof = await _context.PublicProfiles
                       .Include(p => p.Subscribed)
                       .Include(p => p.Following)
                       .Include(p => p.Posts)
                        .ThenInclude(p => p.LikedBy)
                       .Include(p => p.Posts)
                        .ThenInclude(p => p.Comments)
                            .ThenInclude(p => p.CommentBy)
                       .Include(p => p.Posts)
                        .ThenInclude(p => p.Episode)
                    .Where(p => p.UserName == username)
                    .FirstOrDefaultAsync();

                if (prof != null)
                {
                    List<PostApp> profAppPosts = new List<PostApp>();
                    foreach (Post post in prof.Posts)
                    {
                        List<CommentApp> postAppComments = new List<CommentApp>();

                        foreach (Comment comment in post.Comments)
                        {
                            CommentApp c = new CommentApp(comment, await _context.PublicProfiles.FirstOrDefaultAsync
                                (x => x.PublicProfileID.Equals(comment.CommentBy.PublicProfileID)));

                            postAppComments.Add(c);
                        }

                        PostApp postApp = new PostApp(post, prof, postAppComments);
                        profAppPosts.Add(postApp);
                    }

                    PublicProfileApp pApp = new PublicProfileApp(prof, profAppPosts);

                    List<PublicProfile> following = await GetFollowing(prof.Following);
                    // populate following 
                    pApp.Following = following;

                    // May need to iterate through pApp.posts and add this as the poster
                    return Ok(pApp);
                }


                return NoContent();
            }
            catch (Exception)
            {

                return BadRequest();
            }
            

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SubOrUnsubPodcast([FromBody] SubUnsubPodcastBody body)
        {
            string username = body.username;
            int podcastID = int.Parse(body.podcastID);
            bool subOrUnsub = bool.Parse(body.subOrUnsub);

            PublicProfile prof = await _context.PublicProfiles
                   .Include(p => p.Subscribed)
                .Where(p => p.UserName == username)
                .FirstOrDefaultAsync();

            Podcast pod = await _context.Podcasts.FindAsync(podcastID);


            if (prof != null && pod != null)
            {
                if (subOrUnsub)
                {
                    // They want to sub to this podcast
                    prof.Subscribed.Add(pod);
                }
                else
                {
                    // They want to unsubscribe
                    prof.Subscribed.Remove(pod);
                }
                _context.PublicProfiles.Update(prof);
                _context.SaveChanges();

                return Ok();
            }


            return NoContent();
        }

        // GET: api/PublicProfiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublicProfile>>> GetPublicProfiles()
        {
            return await _context.PublicProfiles.ToListAsync();
        }

        // GET: api/PublicProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PublicProfile>> GetPublicProfile(int id)
        {
            var publicProfile = await _context.PublicProfiles.FindAsync(id);

            if (publicProfile == null)
            {
                return NotFound();
            }

            return publicProfile;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetPublicProfilePicture([FromBody] GetPublicProfilePictureBody body)
        {
            int id = int.Parse(body.publicProfileID); 
            var publicProfile = await _context.PublicProfiles.FindAsync(id); 
            if(publicProfile == null)
            {
                return NotFound(); 
            }
            return Ok(publicProfile.PictureLink); 
        }

        // PUT: api/PublicProfiles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublicProfile(int id, PublicProfile publicProfile)
        {
            if (id != publicProfile.PublicProfileID)
            {
                return BadRequest();
            }

            _context.Entry(publicProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublicProfileExists(id))
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

        // POST: api/PublicProfiles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PublicProfile>> PostPublicProfile(PublicProfile publicProfile)
        {
            _context.PublicProfiles.Add(publicProfile);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPublicProfile", new { id = publicProfile.PublicProfileID }, publicProfile);
        }

        // DELETE: api/PublicProfiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublicProfile(int id)
        {
            var publicProfile = await _context.PublicProfiles.FindAsync(id);
            if (publicProfile == null)
            {
                return NotFound();
            }

            _context.PublicProfiles.Remove(publicProfile);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PublicProfileExists(int id)
        {
            return _context.PublicProfiles.Any(e => e.PublicProfileID == id);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetSubscribed([FromBody] GetSubscribedBody body)
        {
            int publicProfileID = int.Parse(body.publicProfileID);

            try
            {
                var subscribed = (await _context.PublicProfiles
                    .Include(x => x.Subscribed)
                    .Where(x => x.PublicProfileID == publicProfileID)
                    .FirstOrDefaultAsync()).Subscribed;

                return Ok(subscribed);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<List<PublicProfile>>> SearchUser([FromBody] SearchUserBody body)
        {
            try
            {
                string searchTerm = body.searchTerm;

                var result = await _context.PublicProfiles
                                    .Include(p => p.Subscribed)
                                    .Include(p => p.Following)
                                    .Include(p => p.Posts)
                                    .Include(p => p.Posts)
                                    .Where(p => p.UserName.Contains(searchTerm))
                                    .Where(p => p.IsPublic == true)
                                    .ToListAsync();

                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SetPrivacy([FromBody] SetPrivacyBody body)
        {
            try
            {
                int pubProfID = int.Parse(body.PublicProfileID);

                var pubProf = await _context.PublicProfiles.FindAsync(pubProfID);

                pubProf.IsPublic = !pubProf.IsPublic; // set privacy to opposite

                _context.Entry(pubProf).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500);
            }
        }

        private async Task<List<PublicProfile>> GetFollowing(List<FollowedBy> followedBy)
        {
            List<PublicProfile> result = new List<PublicProfile>();
            foreach (FollowedBy f in followedBy)
            {
                result.Add((await _context.PublicProfiles
                                            .Include(p => p.Subscribed)
                                            .Include(p => p.Following)
                                            .Include(p => p.Posts)
                                                .ThenInclude(p => p.LikedBy)
                                            .Include(p => p.Posts)
                                                .ThenInclude(p => p.Comments)
                                            .Where(x => x.PublicProfileID == f.FolloweeID)
                                            .FirstOrDefaultAsync()));
            }

            return result;
        }

        public class GetSubscribedBody
        {
            public string publicProfileID { get; set; }
        }

        public class GetOtherUserBody
        {
            public string Username { get; set; }
        }

        public class SearchUserBody
        {
            public string searchTerm { get; set; }
        }
        
        public class SubUnsubPodcastBody
        {
            public string username { get; set; }
            public string podcastID { get; set; }
            public string subOrUnsub { get; set; }
        }

        public class FollowOrUnfollowBody
        {
            public string followeeID { get; set; } // who the current user is following
            public string followerID { get; set; } // the current user's public profile id
        }
        public class SetPrivacyBody
        {
            public string PublicProfileID { get; set; }
        }

        public class UpdateProfilePictureBody
        {
            public string publicProfileID { get; set; }
            public string newPhotoLink { get; set; }

        }
    }

    
    public class GetPublicProfilePictureBody
    {
        public string publicProfileID { get;  set; }
    }
}
