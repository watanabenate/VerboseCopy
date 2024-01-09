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

using Newtonsoft.Json;
using System.ServiceModel.Syndication;
using System.Xml;

namespace VerboseServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly VerboseContext _context;

        public ProfilesController(VerboseContext context)
        {
            _context = context;
        }

        // GET: api/Profiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfiles()
        {
            return await _context.Profiles.ToListAsync();
        }

        // GET: api/Profiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Profile>> GetProfile(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);

            if (profile == null)
            {
                return NotFound();
            }

            return profile;
        }

        // PUT: api/Profiles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfile(int id, Profile profile)
        {
            if (id != profile.ProfileID)
            {
                return BadRequest();
            }

            _context.Entry(profile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(id))
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

        // POST: api/Profiles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Profile>> PostProfile(Profile profile)
        {
            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProfile", new { id = profile.ProfileID }, profile);
        }

        // DELETE: api/Profiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }

            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProfileExists(int id)
        {
            return _context.Profiles.Any(e => e.ProfileID == id);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> VerifyCreator([FromBody] VerifyCreatorBody body)
        {
            string email = body.Email;
            string rssLink = body.RssLink;

            string rssEmail = RssHelper(rssLink);

            if (email.Equals(rssEmail))
            {
                var creator = await _context.Profiles
                                .Where(p => p.Email.Equals(email))
                                .FirstOrDefaultAsync();

                creator.IsCreator = true;
                creator.RssUrl = rssLink;
                _context.SaveChanges();

                return Ok("email verified");
            }
            else
            {
                return Ok("email does not match rss creator email");
            }
        }

        private static string RssHelper(string rssLink)
        {

            using var reader = XmlReader.Create(rssLink);
            var feed = SyndicationFeed.Load(reader);

            var rssEmail = feed.Items.FirstOrDefault().Authors[0].Email;

            string splitRssEmail = rssEmail.Split(" ")[0];

            return splitRssEmail;


        }
    }

    public class VerifyCreatorBody
    {
        public string Email { get; set; }
        public string RssLink { get; set; }
    }
}
