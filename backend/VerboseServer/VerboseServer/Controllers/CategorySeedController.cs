using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerboseServer.Data;
using VerboseServer.Models;

namespace VerboseServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorySeedController : ControllerBase
    {
        private readonly VerboseContext _context;
        public CategorySeedController(VerboseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSeededCategories()
        {
            try
            {
                Dictionary<string, List<Podcast>> result = new Dictionary<string, List<Podcast>>();

                var comedy = await _context.Podcasts
                                    .Include(x => x.Episodes)
                                    .Include(x => x.Categories)
                                    .Where(x => x.Categories.Any(c => c.topic.Equals("Comedy")))
                                    .Take(5)
                                    .ToListAsync();

                result.Add("Comedy", comedy);

               

                var news = await _context.Podcasts
                                    .Include(x => x.Episodes)
                                    .Include(x => x.Categories)
                                    .Where(x => x.Categories.Any(c => c.topic.Equals("News")))
                                    .Take(5)
                                    .ToListAsync();

                result.Add("News", news);


                var sports = await _context.Podcasts
                                    .Include(x => x.Episodes)
                                    .Include(x => x.Categories)
                                    .Where(x => x.Categories.Any(c => c.topic.Equals("Sports")))
                                    .Take(5)
                                    .ToListAsync();

                result.Add("Sports", sports);


                var true_crime = await _context.Podcasts
                                    .Include(x => x.Episodes)
                                    .Include(x => x.Categories)
                                    .Where(x => x.Categories.Any(c => c.topic.Equals("Crime")))
                                    .Take(5)
                                    .ToListAsync();

                result.Add("TrueCrime", true_crime);

                var tv = await _context.Podcasts
                                     .Include(x => x.Episodes)
                                     .Include(x => x.Categories)
                                     .Where(x => x.Categories.Any(c => c.topic.Equals("TV")))
                                     .Take(5)
                                     .ToListAsync();

                result.Add("TV", tv);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
