using System.ComponentModel.DataAnnotations.Schema; 

namespace VerboseServer.Models
{
    public class PublicProfile
    {
        public int PublicProfileID{ get; set; } // Unique profile id
        public string UserName { get; set; } // Username for this user
        public string? PictureLink { get; set; } // Link to their picture
        public bool IsPublic { get; set; } = true;
        public List<FollowedBy> Following { get; set; } = new List<FollowedBy>(); // Users this user is following
        public List<Podcast> Subscribed { get; set; } = new List<Podcast>(); // Podcasts the user is subscribed to
        public List<Post> Posts { get; set; } = new List<Post>();
        public string? googlePictureLink { get; set; } // store original google picture link
    }
}
