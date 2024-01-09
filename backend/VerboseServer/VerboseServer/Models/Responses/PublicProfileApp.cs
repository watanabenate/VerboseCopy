using System.ComponentModel.DataAnnotations.Schema; 

namespace VerboseServer.Models
{
    public class PublicProfileApp
    {

        public PublicProfileApp(PublicProfile p, List<PostApp> posts)
        {
            PublicProfileID = p.PublicProfileID;
            UserName = p.UserName;
            PictureLink = p.PictureLink;
            Subscribed = p.Subscribed;
            Posts = posts;
            IsPublic = p.IsPublic;
            GooglePictureLink = p.googlePictureLink;
        }

        public int PublicProfileID{ get; set; } // Unique profile id
        public string UserName { get; set; } // Username for this user
        public string? PictureLink { get; set; } // Link to their picture
        public string? GooglePictureLink { get; set; }
        public List<PublicProfile> Following { get; set; } = new List<PublicProfile>(); // Users this user is following
        public List<Podcast> Subscribed { get; set; } = new List<Podcast>(); // Podcasts the user is subscribed to
        public List<PostApp> Posts { get; set; } = new List<PostApp>();
        public bool IsPublic { get; set; }
    }
}
