
using System.Collections.Generic;
using Verbose.Data;

namespace Verbose.API
{
    internal class PodcastCategories
    {
        readonly List<string> searchCategories = new List<string>
        {
            "trending",
            "comedy",
            "true_crime",
            "news",
            "sports",
            "lifestyle",
            "business",
            "technology",
            "tv_movie",
            "fiction",
            "celeb",
            "educational"
        };


        Dictionary<string, List<Podcast>> podcastsInCategories;
        public PodcastCategories()
        {
            podcastsInCategories = new Dictionary<string, List<Podcast>>();

            initTrending();

        }

        private void initTrending()
        {
            List<Podcast> trending = new List<Podcast>()
            {
                new Podcast
                {
                    CoverArtLink = "https://www.omnycontent.com/d/playlist/9b7dacdf-a925-4f95-84dc-ac46003451ff/97e061e7-9fab-469b-83a3-acb8002fd07e/bdeb174f-f177-4cc6-a6fb-acb8002fd088/image.jpg?t=1612261084&size=Large",
                    Creator = "Chatty Broads with Bekah and Jess",
                    Episodes = new List<PodcastEpisode>()
                    {
                        new PodcastEpisode
                        {
                            CoverArtLink = "https://www.omnycontent.com/d/playlist/9b7dacdf-a925-4f95-84dc-ac46003451ff/97e061e7-9fab-469b-83a3-acb8002fd07e/bdeb174f-f177-4cc6-a6fb-acb8002fd088/image.jpg?t=1612261084&size=Large",

                        }
                    }

                }
            };

            podcastsInCategories["trending"] = trending;
        }
    }
}