using System.ComponentModel.DataAnnotations;

namespace VerboseServer.Models
{
    public class LikedBy
    {
        [Key]
        public int LikedByID { get; set; }
        public int PublicProfileID { get; set; }
        public int PostID { get; set; }

        public override int GetHashCode()
        {
            return 31 * PublicProfileID.GetHashCode() * PostID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            LikedBy p = (LikedBy)obj;

            if (p.PublicProfileID == this.PublicProfileID
                && p.PostID == this.PostID)
                return true;

            return false;
        }
    }
}
