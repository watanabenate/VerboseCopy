using Microsoft.EntityFrameworkCore;
using VerboseServer.Models;

namespace VerboseServer.Data.Seeding
{
    public static class ProfileSeeding
    {
        public static async Task Seed(VerboseContext context)
        {
            if (context.Profiles.Any())
            {
                return;
            }

            if (context.PublicProfiles.Any())
            {
                return;
            }

            var profiles = new Profile[]
            {
                new Profile
                {
                    Name = "Bruce Wayne",

                    Email = "julia.nguyen76@gmail.com",


                    PublicProfileInfo = new PublicProfile
                    {
                        UserName = "bwayne",
                        Posts = new List<Post>
                        {
                             new Post
                             {
                                Title = "Podcasts are a great way to kill time",
                                Description = "I was doing my rounds and getting rid of all the villains, all the while sharing my favorite podcasts on this app!",
                                Date = DateTime.Now.AddDays(-2),
                                PublicProfileId = 1
                             },

                        },
                        PictureLink = "https://pbs.twimg.com/profile_images/739518032494665730/OrKBpGSF_400x400.jpg",
                        Following = new List<FollowedBy>(),
                        Subscribed = new List<Podcast>(),
                        googlePictureLink = "https://pbs.twimg.com/profile_images/739518032494665730/OrKBpGSF_400x400.jpg"
                    },
                    //RecentlyListenedTo = new List<Episode>
                    //{
                    //    await context.Episodes.FindAsync(3),
                    //    await context.Episodes.FindAsync(5),
                    //    await context.Episodes.FindAsync(12)
                    //},

                },
                new Profile
                {
                    Name = "John Cena",
                    Email = "invisibleman@wrestling.com",
                    PublicProfileInfo =  new PublicProfile
                    {
                        UserName = "jcena",
                        PictureLink = "https://nearshoreamericas.com/wp-content/uploads/2016/04/john-cena-2.jpg",
                        Posts = new List<Post>
                        {
                             new Post
                             {
                                Title = "The Invisible Man",
                                Description = "I love finding new podcasts about me on this app.",
                                Date = DateTime.Now.AddDays(-2),
                                PublicProfileId = 2,
                                Likes = 1,
                                LikedBy = new HashSet<LikedBy>
                                {
                                    new LikedBy
                                    {
                                        PostID = 1,
                                        PublicProfileID = 1
                                    }
                                }
                             },

                        }
                    }

                },
                new Profile
                {
                    Name = "Carl Wheezer",
                    Email = "cwheezer@hotmail.com",
                    PublicProfileInfo =  new PublicProfile
                    {
                        UserName = "cwheezer",
                        PictureLink = "https://pbs.twimg.com/profile_images/1477467455874379777/hvgP_MW3_400x400.jpg",
                        Posts = new List<Post>
                        {
                            new Post
                            {
                                Title = "Verbose is so cool!",
                                Description = "This app really helps me share and listen to my favorite podcasts",
                                Date = DateTime.Now.AddDays(-3),
                                PublicProfileId=3
                            },
                            new Post
                            {
                                Title = "Podcasts...",
                                Description = "Who else listens to them while showering???",
                                Date = DateTime.Now.AddDays(-1),
                                PublicProfileId = 3
                            }
                        }
                    }
                },
                new Profile
                {
                    Name = "Dwayne The Rock Johnson",
                    Email = "justtherockjohnson@gmail.com",
                    PublicProfileInfo =  new PublicProfile
                    {
                        UserName = "dtrjohnson",
                    }
                },
                new Profile
                {
                    Name = "Tyler Durden",
                    Email = "tdurden@msn.com",
                    PublicProfileInfo = new PublicProfile
                    {
                        UserName = "tdurden",
                    }
                },
                new Profile
                {
                    Name = "Will Smith",
                    Email = "wsmith@gmail.com",
                    PublicProfileInfo = new PublicProfile
                    {
                        UserName = "cwheezer1"
                    }
                },
                new Profile
                {
                    Name = "Tay Zonday",
                    Email = "tzonday@gmail.com",
                    PublicProfileInfo = new PublicProfile
                    {
                        UserName = "bwaynee"
                    }
                },
                new Profile
                {
                    Name = "Dick Grayson",
                    Email = "dgrayson@gmail.com",
                    PublicProfileInfo = new PublicProfile
                    {
                        UserName = "bwayne2"
                    }
                }
        };

            foreach (Profile p in profiles)
            {
                context.Profiles.Add(p);
            }


            context.SaveChanges();

            // add following
            var bwayne = context.PublicProfiles.Include(p => p.Following).Include(p => p.Subscribed).Where(p => p.UserName.Equals("bwayne")).FirstOrDefault();
            bwayne.Following = new List<FollowedBy>
            {
                //await context.PublicProfiles.FindAsync(2),
                //await context.PublicProfiles.FindAsync(3)
                new FollowedBy
                {
                    FollowerID = bwayne.PublicProfileID, 
                    FolloweeID = 2
                },
                 new FollowedBy
                {
                    FollowerID = bwayne.PublicProfileID,
                    FolloweeID = 3
                },

            };

            var jcena = context.PublicProfiles.Include(p => p.Following).Include(p => p.Subscribed).Where(p => p.UserName.Equals("jcena")).FirstOrDefault();

            List<Podcast> jcenaPodcast = new List<Podcast>();
            jcenaPodcast.Add(context.Podcasts.First());
            jcena.Subscribed = jcenaPodcast;

            context.SaveChanges();
        }
    }
}