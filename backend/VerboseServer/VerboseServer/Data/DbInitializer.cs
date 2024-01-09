using VerboseServer.Models;
using System;
using System.Linq;
using VerboseServer.Data.Seeding;

namespace VerboseServer.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(VerboseContext context)
        {
            context.Database.EnsureCreated();

            //EpisodeSeeding.Seed(context);
            //SearchResults.Seed(context);
            await CategorySeeding.Seed(context);
            await ProfileSeeding.Seed(context);
           
        }
    }
}
