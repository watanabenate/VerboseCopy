//using VerboseServer.Models;
//namespace VerboseServer.Data.Seeding
//{
//    public static class EpisodeSeeding
//    {
//        public static void Seed (VerboseContext context)
//        {
//            if (context.Episodes.Any())
//            {
//                return;
//            }

//            var episodes = new Episode[]{
//                new Episode{
//                    Title="S3E19 Prom-asaurus ft. Tiffani Allen",
//                    Creator="Ian Allred & Lena Conatser",
//                    CoverArtLink="https://d3t3ozftmdmh3i.cloudfront.net/production/podcast_uploaded_nologo400/8458549/8458549-1598346667063-ccdb1a97726a5.jpg",
//                    PlayLink="https://anchor.fm/s/33034f34/podcast/play/43443249/sponsor/a4a9m37/https%3A%2F%2Fd3ctxlq1ktw2nl.cloudfront.net%2Fstaging%2F2021-11-16%2F2680ca6b965bac8212e1f507e66b6ad2.m4a",
//                    Comments = new List<Comment>(),
//                    LikeCount = 0,
//                    LikedBy = new HashSet<PublicProfile>(),
//                },
//                new Episode
//                {
//                    Title="Life Lessons with Karama Horne and Ijeoma Njaka",
//                    Creator="iHeartRadio and Nickelodeon",
//                    CoverArtLink="https://megaphone.imgix.net/podcasts/ed6c7efa-bf16-11eb-a21f-77acf4ec2830/image/AVATAR_Final_3000x3000_iHR.png?ixlib=rails-2.1.2&max-w=3000&max-h=3000&fit=crop&auto=format",
//                    PlayLink="https://www.podtrac.com/pts/redirect.mp3/chtbl.com/track/5899E/traffic.megaphone.fm/HSW7880325841.mp3?updated=1638263120",
//                    Comments = new List<Comment>(),
//                    LikeCount = 0,
//                    LikedBy = new HashSet<PublicProfile>(),
//                },
//                new Episode
//                {
//                    Title="Heavy Competition",
//                    Creator="Office Ladies",
//                    CoverArtLink="https://image.simplecastcdn.com/images/eeecdf60-9801-4195-90c3-84742003404a/744ba99d-a29f-4567-a058-15727130fcbe/3000x3000/image.jpg?aid=rss_feed",
//                    PlayLink="https://pdst.fm/e/stitcher.simplecastaudio.com/eeecdf60-9801-4195-90c3-84742003404a/episodes/48d40d3c-bc70-4f17-988e-995905d6236e/audio/128/default.mp3?aid=rss_feed&awCollectionId=eeecdf60-9801-4195-90c3-84742003404a&awEpisodeId=48d40d3c-bc70-4f17-988e-995905d6236e&feed=PxEW_ipK",
//                    Comments = new List<Comment>(),
//                    LikeCount = 0,
//                    LikedBy = new HashSet<PublicProfile>(),
//                },
//                new Episode
//                { 
//                    Title= "The Always Sunny Podcast", 
//                    Creator="Charlie Day, Glenn Howerton, Rob McElhenney",
//                    CoverArtLink="https://image.simplecastcdn.com/images/4d6b1a9f-a5bd-4e5e-9e41-2c726ffd279b/15e25a62-9a91-4d5c-844a-d3cbae2223c8/3000x3000/the-always-sunny-podcast-image.jpg?aid=rss_feed", 
//                    PlayLink="https://cdn.simplecast.com/audio/31e190a7-769e-4d96-8175-7fecd41336f8/episodes/30112a5c-a1a5-414a-b0aa-b9ac0dfedfd4/audio/66516223-3f2b-4477-a160-8f227e88efba/default_tc.mp3?aid=rss_feed&feed=eoftDcQK",
//                    Comments = new List<Comment>(),
//                    LikeCount = 0,
//                    LikedBy = new HashSet<PublicProfile>(),
//                },
//            };
//            foreach (Episode e in episodes)
//            {
//                context.Episodes.Add(e);
//            }
//            context.SaveChanges();
//        }
//    }
//}
