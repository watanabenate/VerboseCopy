using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Java.Interop;
using System.Collections.Generic;
using System.Linq;

namespace Verbose.Data
{
    public class PodcastEpisodeParcelable : Java.Lang.Object, IParcelable
    {
        public PodcastEpisode episode { get; set; }

        public PodcastEpisodeParcelable()
        {

        }

        public PodcastEpisodeParcelable(PodcastEpisode e)
        {
            episode = e;
        }

        private PodcastEpisodeParcelable(Parcel parcel)
        {
            // Read in our list of comments first
            List<Comment> comments = new List<Comment>();
            List<CommentParcelable> commentsParcelable = new List<CommentParcelable>(); 
            parcel.ReadParcelableList(commentsParcelable, Java.Lang.Class.FromType(typeof(CommentParcelable)).ClassLoader);
            foreach(CommentParcelable c in commentsParcelable)
            {
                comments.Add(c.comment);
            }

            // Public profile likedby
            HashSet<int> likedBy = new HashSet<int>();
            List<int> likedByParcelable = new List<int>();
            parcel.ReadList(likedByParcelable, Java.Lang.Class.FromType(typeof(int)).ClassLoader);

            episode = new PodcastEpisode
            {
                Title = parcel.ReadString(),
                Creator = parcel.ReadString(),
                PlayLink = parcel.ReadString(),
                CoverArtLink = parcel.ReadString(),
                Description = parcel.ReadString(), 
                EpisodeID = parcel.ReadInt(),
                LikeCount = parcel.ReadInt(),
                PodchaserPodcastID = parcel.ReadInt(),
                Comments = comments,
                CoverArtImage = null,
                LikedBy = likedBy,
            };
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            if(episode == null) { return; }
            if(episode.Comments == null) { episode.Comments = new List<Comment>(); }
            if (episode.LikedBy == null) { episode.LikedBy = new HashSet<int>(); }

            List<CommentParcelable> comments = new List<CommentParcelable>();
            foreach(Comment c in episode.Comments)
            {
                comments.Add(new CommentParcelable(c));
            }
            dest.WriteParcelableList(comments, 0);

            dest.WriteList(episode.LikedBy.ToList());

            dest.WriteString(episode.Title);
            dest.WriteString(episode.Creator);
            dest.WriteString(episode.PlayLink);
            dest.WriteString(episode.CoverArtLink);
            dest.WriteString(episode.Description);

            dest.WriteInt(episode.EpisodeID);
            dest.WriteInt(episode.LikeCount);
            dest.WriteInt(episode.PodchaserPodcastID);

        }

        private static readonly GenericParcelableCreator<PodcastEpisodeParcelable> _creator
            = new GenericParcelableCreator<PodcastEpisodeParcelable>((parcel) => new PodcastEpisodeParcelable(parcel));

        [ExportField("CREATOR")]
        public static GenericParcelableCreator<PodcastEpisodeParcelable> GetCreator()
        {
            return _creator;
        }

    }
}