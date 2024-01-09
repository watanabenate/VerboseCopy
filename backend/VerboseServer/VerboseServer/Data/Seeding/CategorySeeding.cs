using Newtonsoft.Json;
using VerboseServer.Models;

namespace VerboseServer.Data.Seeding
{
    public class CategorySeeding
    {
        public static async Task Seed(VerboseContext context)
        {
            if (context.Podcasts.Any())
            {
                return;
            }

            var comedy = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText("MockData/comedy.txt"));
            var news = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText("MockData/news.txt"));
            var sports = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText("MockData/sports.txt"));
            var true_crime = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText("MockData/true_crime.txt"));
            var tv = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText("MockData/tv.txt"));

            foreach (Datum PodchaserPodcast in comedy.data.podcasts.data)
            {
                if ((await context.Podcasts.FindAsync(int.Parse(PodchaserPodcast.id))) == null)
                {
                    List<PodcastCategory> categoryList = new List<PodcastCategory>();
                    foreach (Category topic in PodchaserPodcast.categories)
                    {
                        categoryList.Add( new PodcastCategory { topic = topic.title });
                    }

                    Podcast p = new Podcast
                    {
                        PodcastID = int.Parse(PodchaserPodcast.id),
                        Creator = PodchaserPodcast.author.name,
                        CoverArtLink = PodchaserPodcast.imageUrl,
                        Title = PodchaserPodcast.title,
                        Description = PodchaserPodcast.description,
                        Categories = categoryList,
                        Episodes = AddEpisodes(PodchaserPodcast.episodes, int.Parse(PodchaserPodcast.id), PodchaserPodcast.author.name)
                    };

                    context.Podcasts.Add(p);
                }
            }

            foreach (Datum PodchaserPodcast in news.data.podcasts.data)
            {
                if ((await context.Podcasts.FindAsync(int.Parse(PodchaserPodcast.id))) == null)
                {
                    List<PodcastCategory> categoryList = new List<PodcastCategory>();
                    foreach (Category topic in PodchaserPodcast.categories)
                    {
                        categoryList.Add(new PodcastCategory { topic = topic.title });
                    }

                    Podcast p = new Podcast
                    {
                        PodcastID = int.Parse(PodchaserPodcast.id),
                        Creator = PodchaserPodcast.author.name,
                        CoverArtLink = PodchaserPodcast.imageUrl,
                        Title = PodchaserPodcast.title,
                        Description = PodchaserPodcast.description,
                        Categories = categoryList,
                        Episodes = AddEpisodes(PodchaserPodcast.episodes, int.Parse(PodchaserPodcast.id), PodchaserPodcast.author.name)
                    };

                    context.Podcasts.Add(p);
                }
            }

            foreach (Datum PodchaserPodcast in sports.data.podcasts.data)
            {
                if ((await context.Podcasts.FindAsync(int.Parse(PodchaserPodcast.id))) == null)
                {
                    List<PodcastCategory> categoryList = new List<PodcastCategory>();
                    foreach (Category topic in PodchaserPodcast.categories)
                    {
                        categoryList.Add(new PodcastCategory { topic = topic.title });
                    }

                    Podcast p = new Podcast
                    {
                        PodcastID = int.Parse(PodchaserPodcast.id),
                        Creator = PodchaserPodcast.author.name,
                        CoverArtLink = PodchaserPodcast.imageUrl,
                        Title = PodchaserPodcast.title,
                        Description = PodchaserPodcast.description,
                        Categories = categoryList,
                        Episodes = AddEpisodes(PodchaserPodcast.episodes, int.Parse(PodchaserPodcast.id), PodchaserPodcast.author.name)
                    };

                    context.Podcasts.Add(p);
                }
            }

            foreach (Datum PodchaserPodcast in true_crime.data.podcasts.data)
            {
                if ((await context.Podcasts.FindAsync(int.Parse(PodchaserPodcast.id))) == null)
                {
                    List<PodcastCategory> categoryList = new List<PodcastCategory>();
                    foreach (Category topic in PodchaserPodcast.categories)
                    {
                        categoryList.Add(new PodcastCategory { topic = topic.title });
                    }

                    Podcast p = new Podcast
                    {
                        PodcastID = int.Parse(PodchaserPodcast.id),
                        Creator = PodchaserPodcast.author.name,
                        CoverArtLink = PodchaserPodcast.imageUrl,
                        Title = PodchaserPodcast.title,
                        Description = PodchaserPodcast.description,
                        Categories = categoryList,
                        Episodes = AddEpisodes(PodchaserPodcast.episodes, int.Parse(PodchaserPodcast.id), PodchaserPodcast.author.name)
                    };

                    context.Podcasts.Add(p);
                }
            }

            foreach (Datum PodchaserPodcast in tv.data.podcasts.data)
            {
                if ((await context.Podcasts.FindAsync(int.Parse(PodchaserPodcast.id))) == null)
                {
                    List<PodcastCategory> categoryList = new List<PodcastCategory>();
                    foreach (Category topic in PodchaserPodcast.categories)
                    {
                        categoryList.Add(new PodcastCategory { topic = topic.title });
                    }

                    Podcast p = new Podcast
                    {
                        PodcastID = int.Parse(PodchaserPodcast.id),
                        Creator = PodchaserPodcast.author.name,
                        CoverArtLink = PodchaserPodcast.imageUrl,
                        Title = PodchaserPodcast.title,
                        Description = PodchaserPodcast.description,
                        Categories = categoryList,
                        Episodes = AddEpisodes(PodchaserPodcast.episodes, int.Parse(PodchaserPodcast.id), PodchaserPodcast.author.name)
                    };

                    context.Podcasts.Add(p);
                }
            }

            context.SaveChanges();


        }

        private static List<Episode> AddEpisodes(Episodes ea, int PodcastID, string creator)
        {
            List<Episode> EpisodeList = new List<Episode>();
            foreach (Datum1 pe in ea.data)
            {
                Episode e = new Episode
                {
                    PodchaserPodcastID = PodcastID,
                    EpisodeID = int.Parse(pe.id),
                    Description = pe.description,
                    Title = pe.title,
                    PlayLink = pe.audioUrl,
                    CoverArtLink = pe.imageUrl,
                    Creator = creator,
                };
                EpisodeList.Add(e);
            }
            return EpisodeList;
        }

        public class Rootobject
        {
            public Data data { get; set; }
        }

        public class Data
        {
            public Podcasts podcasts { get; set; }
        }

        public class Podcasts
        {
            public Datum[] data { get; set; }
        }

        public class Datum
        {
            public string id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string imageUrl { get; set; }
            public Author author { get; set; }
            public Category[] categories { get; set; }
            public Episodes episodes { get; set; }
        }

        public class Author
        {
            public string name { get; set; }
        }

        public class Episodes
        {
            public Datum1[] data { get; set; }
        }

        public class Datum1
        {
            public string id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string imageUrl { get; set; }
            public string audioUrl { get; set; }
        }

        public class Category
        {
            public string title { get; set; }
        }

    }

}
