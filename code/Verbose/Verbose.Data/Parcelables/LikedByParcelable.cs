using Android.OS;
using Android.Runtime;
using Java.Interop;
using System;
using System.Collections.Generic;

namespace Verbose.Data
{
    public class LikedByParcelable : Java.Lang.Object, IParcelable
    {
        public LikedBy likedBy { get; set; }

        public LikedByParcelable()
        {

        }

        public LikedByParcelable(LikedBy l)
        {
            likedBy = l;
        }

        private LikedByParcelable(Parcel parcel)
        {
            likedBy = new LikedBy
            {
                PostID = parcel.ReadInt(),
                PublicProfileID = parcel.ReadInt(),
                LikedByID = parcel.ReadInt(),
            };
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            if(likedBy == null) { return; }

            dest.WriteInt(likedBy.PostID);
            dest.WriteInt(likedBy.PublicProfileID);
            dest.WriteInt(likedBy.LikedByID);
        }

        private static readonly GenericParcelableCreator<LikedByParcelable> _creator
            = new GenericParcelableCreator<LikedByParcelable>((parcel) => new LikedByParcelable(parcel));

        [ExportField("CREATOR")]
        public static GenericParcelableCreator<LikedByParcelable> GetCreator()
        {
            return _creator;
        }

    }
}