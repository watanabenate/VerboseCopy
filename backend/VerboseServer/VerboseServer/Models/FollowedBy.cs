namespace VerboseServer.Models
{
    public class FollowedBy
    {
        public int FollowedByID { get; set; }
        // user that's following the person
        public int FollowerID { get; set; }
        // the person that the user is following 
        public int FolloweeID { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            FollowedBy f = (FollowedBy)obj;

            if (f.FollowerID == this.FollowerID
                && f.FolloweeID == this.FolloweeID)
                return true;

            return false;
        }
    }
}
