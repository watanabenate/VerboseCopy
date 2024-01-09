namespace VerboseServer.Models
{
    public class Post
    {
        public int PostID { get; set; }
        public string? ImageURL { get; set; }    
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Comment> Comments { get; set; }
        public DateTime Date { get; set; }
        public int Likes { get; set; }
        public Episode? Episode { get; set; } // Each post connects to an episode

        public ICollection<LikedBy> LikedBy { get; set; } = new HashSet<LikedBy>();
        
        public int PublicProfileId { get; set; } // id of the poster 
    }
}
