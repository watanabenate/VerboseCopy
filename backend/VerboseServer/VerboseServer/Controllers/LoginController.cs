using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VerboseServer.Data;
using VerboseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace VerboseServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly VerboseContext _context;

        public LoginController(VerboseContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CheckAccountCreated([FromBody] CheckAccountCreatedBody body)
        {
            string email = body.Email;

            var prof = await _context.Profiles
                .Include(p => p.PublicProfileInfo)
                    .ThenInclude(p => p.Subscribed)
                .Include(p => p.PublicProfileInfo)
                    .ThenInclude(p => p.Following)
                .Include(p => p.PublicProfileInfo)
                    .ThenInclude(p => p.Posts)
                    .ThenInclude(p => p.LikedBy)
                    .Include(p => p.PublicProfileInfo)
                    .ThenInclude(p => p.Posts)
                    .ThenInclude(p => p.Comments)
                .Where(p => p.Email == email)
                .FirstOrDefaultAsync();

            if (prof != null)
            {
                List<PostApp> profAppPosts = new List<PostApp>();

                PublicProfileApp pApp = new PublicProfileApp(prof.PublicProfileInfo, profAppPosts);

                List<PublicProfile> following = await GetFollowing(prof.PublicProfileInfo.Following);
                // populate following 
                pApp.Following = following;

                ProfileApp profileApp = new ProfileApp(prof, pApp);

                // May need to iterate through pApp.posts and add this as the poster
                return Ok(profileApp);
            }
                

            return NoContent();

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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateNewUser([FromBody] CreateNewUserBody body)
        {
            string email = body.Email;
            string fullName = body.FullName;
            string pictureLink = body.PictureLink;
            string userName = body.UserName;
            string googleLink = body.PictureLink; 


            Profile newProfile = new Profile
            {
                Email = email,
                Name = fullName,
                PublicProfileInfo = new PublicProfile
                {
                    UserName = userName,
                    PictureLink = pictureLink,
                    googlePictureLink = googleLink,
                }
            };

            await _context.Profiles.AddAsync(newProfile);
            _context.SaveChanges();

            return Ok(newProfile);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CheckUserNameAvailable([FromBody] CheckUserNameBody body)
        {
            try
            {


                string userName = body.UserName;

                var user = await _context.PublicProfiles
                    .Where(u => u.UserName == userName)
                    .FirstOrDefaultAsync();
                // If it's empty, they can use that name
                if (user == null)
                    return Ok();
                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            

        }
    }

    public class CheckAccountCreatedBody
    {
        public string Email { get; set; }
    }

    public class CreateNewUserBody
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PictureLink { get; set; }
        public string UserName { get; set; }
    }

    public class CheckUserNameBody
    {
        public string UserName { get; set; }
    }

}
