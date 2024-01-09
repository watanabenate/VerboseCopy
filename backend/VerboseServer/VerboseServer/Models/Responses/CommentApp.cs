
namespace VerboseServer.Models
{
    public class CommentApp
    {
        public CommentApp(Comment c, PublicProfile p)
        {
            CommentID = c.CommentID;   
            Text = c.Text;
            Likes = c.Likes;
            Date = c.Date;
            Timestamp = c.Timestamp;
            EpisodeID = c?.EpisodeID;
            PostID = c?.PostID;

            Username = p.UserName;
            ProfileImageLink = p?.PictureLink;
            ProfileID = p.PublicProfileID;
        }

        public int CommentID { get; set; }
        public string Text { get; set;  }
        public int Likes { get; set; }
        public DateTime Date { get; set; }
        public long Timestamp { get; set; }
        public int? EpisodeID { get; set; }
        public int? PostID { get; set; }
        public string Username { get; set; }
        public string ProfileImageLink { get; set; }
        public int ProfileID { get; set; }
    }
}
