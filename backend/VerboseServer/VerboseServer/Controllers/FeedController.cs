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
    public class FeedController : ControllerBase
    {
        private readonly VerboseContext _context;

        public FeedController(VerboseContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<IEnumerable<ListenedToApp>>> GetRecentlyListenedTo([FromBody] GetRecentlyListenedToBody body)
        {
            try
            {


                string username = body.Username;

                var pub = await _context.PublicProfiles
                    .FirstOrDefaultAsync(x => x.UserName.Equals(username));

                var recentlyListenedTo = (await _context.Profiles
                    .Include(x => x.RecentlyListenedTo)
                    .FirstOrDefaultAsync(x => x.PublicProfileID == pub.PublicProfileID)).RecentlyListenedTo.OrderByDescending(x => x.DateListened);

                List<ListenedToApp> result = new List<ListenedToApp>();

                foreach (ListenedTo l in recentlyListenedTo)
                {
                    var episode =
                        await _context.Episodes
                        .Include(x => x.Comments).ThenInclude(x => x.CommentBy)
                        .Include(x => x.LikedBy).FirstOrDefaultAsync(x => x.EpisodeID.Equals(l.EpisodeID));

                    if(episode == null) continue;

                    List<CommentApp> episodeAppComments = new List<CommentApp>();

                    foreach (Comment comment in episode.Comments)
                    {
                        CommentApp c = new CommentApp(comment, await _context.PublicProfiles.FirstOrDefaultAsync
                            (x => x.PublicProfileID.Equals(comment.CommentBy.PublicProfileID)));
                        
                        episodeAppComments.Add(c);  
                    }

                    EpisodeApp e = new EpisodeApp(episode, episodeAppComments);

                    result.Add(new ListenedToApp(e, l.Timestamp));
                }

                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(); 
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<IEnumerable<Episode>>> RefreshUserPosts([FromBody] RefreshUserPostsBody body)
        {
            int index = int.Parse(body.index);
            string username = body.Username;

            List<PostApp> userProfilePosts = new List<PostApp>();

            var userPosts = (await _context.PublicProfiles
                .Include(x => x.Posts).ThenInclude(x => x.LikedBy)
                .Include(x => x.Posts).ThenInclude(x => x.Comments).ThenInclude(x => x.CommentBy)
                .Include(x => x.Posts).ThenInclude(x => x.Episode)
                .Where(x => x.UserName.Equals(username))
                .FirstOrDefaultAsync()).Posts;

            PublicProfile userProfile = await _context.PublicProfiles
                .Include(x => x.Posts).ThenInclude(x => x.LikedBy)
                .Where(x => x.UserName == username)
                .FirstOrDefaultAsync();

            foreach (Post post in userPosts)
            {
                List<CommentApp> postAppComments = new List<CommentApp>();

                foreach (Comment comment in post.Comments)
                {
                    CommentApp c = new CommentApp(comment, await _context.PublicProfiles.FirstOrDefaultAsync
                        (x => x.PublicProfileID.Equals(comment.CommentBy.PublicProfileID)));

                    postAppComments.Add(c);
                }

                PostApp postApp = new PostApp(post, userProfile, postAppComments);
                userProfilePosts.Add(postApp);
            }

            var orderedPosts = userProfilePosts
                .OrderByDescending(x => x.Date)
                .Skip(index)/*.Take(20)*/;

            return Ok(orderedPosts);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<IEnumerable<Episode>>> GetMainFeedPosts([FromBody] GetMainFeedPostsBody body)
        {
            int index = int.Parse(body.index);
            string username = body.Username;

            var following = (await _context.PublicProfiles
                .Where(x => x.UserName.Equals(username))
                .Include(x => x.Following)
                .FirstOrDefaultAsync()).Following;

            var posts = new List<PostApp>();

            int counter = index;

            foreach (FollowedBy p in following)
            {
                
                var PPosts = (await _context.PublicProfiles
                    .Include(x => x.Posts).ThenInclude(x => x.LikedBy)
                    .Include(x => x.Posts).ThenInclude(x => x.Episode)
                    .Include(x => x.Posts).ThenInclude(x => x.Comments).ThenInclude(x => x.CommentBy)
                    .FirstOrDefaultAsync(x => x.PublicProfileID == p.FolloweeID))
                    .Posts;

                foreach (Post post in PPosts)
                {
                    List<CommentApp> postAppComments = new List<CommentApp>();

                    foreach (Comment comment in post.Comments)
                    {
                        CommentApp c = new CommentApp(comment, await _context.PublicProfiles.FirstOrDefaultAsync
                            (x => x.PublicProfileID.Equals(comment.CommentBy.PublicProfileID)));

                        postAppComments.Add(c);
                    }

                    PostApp postApp = new PostApp(post, await _context.PublicProfiles.FindAsync(p.FolloweeID), postAppComments);

                    posts.Add(postApp);
                }
            }

            var userPosts = (await _context.PublicProfiles
                .Include(x => x.Posts).ThenInclude(x => x.LikedBy)
                .Include(x => x.Posts).ThenInclude(x => x.Episode)
                .Include(x => x.Posts).ThenInclude(x => x.Comments).ThenInclude(x => x.CommentBy)
                .Where(x => x.UserName.Equals(username))
                .FirstOrDefaultAsync()).Posts;

            PublicProfile userProfile = await _context.PublicProfiles
                .Include(x => x.Posts).ThenInclude(x => x.LikedBy)
                .Where(x => x.UserName == username)
                .FirstOrDefaultAsync();

            foreach (Post post in userPosts)
            {
                List<CommentApp> postAppComments = new List<CommentApp>();

                foreach (Comment comment in post.Comments)
                {
                    CommentApp c = new CommentApp(comment, await _context.PublicProfiles.FirstOrDefaultAsync
                        (x => x.PublicProfileID.Equals(comment.CommentBy.PublicProfileID)));

                    postAppComments.Add(c);
                }

                PostApp postApp = new PostApp(post, userProfile, postAppComments);
                posts.Add(postApp);
            }

            var orderedPosts = posts
                .OrderByDescending(x => x.Date)
                .Skip(index).Take(20);

            return Ok(orderedPosts);
        }
    }

    public class GetRecentlyListenedToBody
    {
        public string Username { get; set; }
    }

    public class RefreshUserPostsBody
    {
        public string index { get; set; }
        public string Username { get; set; }
    }

    public class GetMainFeedPostsBody
    {
        public string index { get; set; }
        public string Username { get; set; }
    }

}

