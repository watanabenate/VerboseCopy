
namespace VerboseServer.Models
{
    public class Comment
    {
        public int CommentID { get; set; }
        public string Text { get; set;  }
        public CommentBy CommentBy { get; set; }
        public int Likes { get; set; }
        public DateTime Date { get; set; }
        public long Timestamp { get; set; }
        public List<Comment> Replies { get; set; }
        public int? EpisodeID { get; set; }
        public int? PostID { get; set; }
    }
}
