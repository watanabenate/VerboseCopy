namespace VerboseServer.Models
{
    public class CommentBy
    {
        public int CommentByID { get; set; }
        public int CommentID { get; set; }
        public int PublicProfileID { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            CommentBy c = (CommentBy)obj;

            if (c.PublicProfileID == this.PublicProfileID
                && c.CommentID == this.CommentID)
                return true;

            return false;
        }
    }
}
