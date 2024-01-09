using Android.OS;
using Android.Runtime;
using Java.Interop;
using System;
using System.Collections.Generic;

namespace Verbose.Data
{
    public class PostParcelable : Java.Lang.Object, IParcelable
    {
        public Post p { get; set; }

        public PostParcelable()
        {

        }

        public PostParcelable(Post post)
        {
            p = post;
        }

        private PostParcelable(Parcel parcel)
        {
            // Read in our list of comments first
            List<Comment> comments = new List<Comment>();
            List<CommentParcelable> commentsParcelable = new List<CommentParcelable>();
            parcel.ReadParcelableList(commentsParcelable, Java.Lang.Class.FromType(typeof(CommentParcelable)).ClassLoader);
            foreach (CommentParcelable c in commentsParcelable)
            {
                comments.Add(c.comment);
            }

            // likedby
            HashSet<LikedBy> likedBy = new HashSet<LikedBy>();
            List<LikedByParcelable> likedByParcelable = new List<LikedByParcelable>();
            parcel.ReadParcelableList(likedByParcelable, Java.Lang.Class.FromType(typeof(LikedByParcelable)).ClassLoader);
            foreach (LikedByParcelable lb in likedByParcelable)
            {
                likedBy.Add(lb.likedBy);
            }

            p = new Post
            {
                ImageURL = parcel.ReadString(),
                Title = parcel.ReadString(),
                Description = parcel.ReadString(),
                Date = DateTime.Parse(parcel.ReadString()),
                Likes = parcel.ReadInt(),
                Episode = ((PodcastEpisodeParcelable) parcel.ReadParcelable(Java.Lang.Class.FromType(typeof(PodcastEpisodeParcelable)).ClassLoader)).episode,
                Comments = comments,
                LikedBy = likedBy,
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
            if(p == null) { return; }
            if(p.Comments == null) { p.Comments = new List<Comment>(); }
            if(p.LikedBy == null) { p.LikedBy = new HashSet<LikedBy>(); }

            List<CommentParcelable> comments = new List<CommentParcelable>();
            foreach (Comment c in p.Comments)
            {
                comments.Add(new CommentParcelable(c));
            }
            dest.WriteParcelableList(comments, 0);

            List<LikedByParcelable> likedByList = new List<LikedByParcelable>();
            foreach (LikedBy lb in p.LikedBy)
            {
                likedByList.Add(new LikedByParcelable(lb));
            }
            dest.WriteParcelableList(likedByList, 0);

            dest.WriteString(p.ImageURL);
            dest.WriteString(p.Title);
            dest.WriteString(p.Description);
            dest.WriteString(p.Date.ToString());
            dest.WriteInt(p.Likes);
            dest.WriteParcelable(new PodcastEpisodeParcelable(p.Episode), 0);
            dest.WriteString(p.Username);
            dest.WriteString(p.ProfileImageLink);
            dest.WriteInt(p.ProfileID);

        }

        private static readonly GenericParcelableCreator<PostParcelable> _creator
            = new GenericParcelableCreator<PostParcelable>((parcel) => new PostParcelable(parcel));

        [ExportField("CREATOR")]
        public static GenericParcelableCreator<PostParcelable> GetCreator()
        {
            return _creator;
        }

    }
}