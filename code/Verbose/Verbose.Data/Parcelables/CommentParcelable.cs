using Android.OS;
using Android.Runtime;
using Java.Interop;
using System;
using System.Collections.Generic;

namespace Verbose.Data
{
    public class CommentParcelable : Java.Lang.Object, IParcelable
    {
        public Comment comment { get; set; }

        public CommentParcelable()
        {

        }

        public CommentParcelable(Comment c)
        {
            comment = c;
        }

        private CommentParcelable(Parcel parcel)
        {
            comment = new Comment
            {
                Text = parcel.ReadString(),
                Likes = parcel.ReadInt(),
                Date = DateTime.Parse(parcel.ReadString()),
                Timestamp = parcel.ReadLong(),
                Username = parcel.ReadString(),
                ProfileImageLink = parcel.ReadString(),
                ProfileID = parcel.ReadInt(),
            };
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            if(comment == null) { return; }

            dest.WriteString(comment.Text);
            dest.WriteInt(comment.Likes);
            dest.WriteString(comment.Date.ToString());
            dest.WriteLong(comment.Timestamp);
            dest.WriteString(comment.Username);
            dest.WriteString(comment.ProfileImageLink);
            dest.WriteInt(comment.ProfileID);
            // TODO: Write replies???
    }

        private static readonly GenericParcelableCreator<CommentParcelable> _creator
            = new GenericParcelableCreator<CommentParcelable>((parcel) => new CommentParcelable(parcel));

        [ExportField("CREATOR")]
        public static GenericParcelableCreator<CommentParcelable> GetCreator()
        {
            return _creator;
        }

    }
}