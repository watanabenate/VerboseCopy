namespace VerboseServer.Models
{
    public class PostApp
    {
        public PostApp(Post p, PublicProfile publicProfile, List<CommentApp> comments)
        {
            PostID = p.PostID;
            ImageURL = p?.ImageURL;
            Title = p.Title;
            Description = p.Description;
            Comments = comments;
            Date = p.Date;
            Likes = p.Likes;
            Episode = p?.Episode;
            LikedBy = p.LikedBy;
            Username = publicProfile.UserName;
            ProfileImageLink = publicProfile?.PictureLink;
            ProfileID = publicProfile.PublicProfileID;
        }

        public int PostID { get; set; }
        public string? ImageURL { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<CommentApp> Comments { get; set; }
        public DateTime Date { get; set; }
        public int Likes { get; set; }
        public Episode? Episode { get; set; } // Each post connects to an episode
        public ICollection<LikedBy> LikedBy { get; set; } = new HashSet<LikedBy>();
        public string Username { get; set; }
        public string ProfileImageLink { get; set; }
        public int ProfileID { get; set; }
    }
}
