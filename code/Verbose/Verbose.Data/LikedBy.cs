using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Verbose.Data
{
    public class LikedBy
    {
        public int LikedByID;
        public int PublicProfileID;
        public int PostID;

        public override int GetHashCode()
        {
            return 31 * PublicProfileID.GetHashCode();
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