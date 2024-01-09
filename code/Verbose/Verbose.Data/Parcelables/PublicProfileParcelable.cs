using Android.OS;
using Android.Runtime;
using Java.Interop;
using System.Collections.Generic;

namespace Verbose.Data
{
    public class PublicProfileParcelable : Java.Lang.Object, IParcelable
    {
        public PublicProfile pp { get; set; }

        public PublicProfileParcelable()
        {

        }

        public PublicProfileParcelable(PublicProfile pp)
        {
            this.pp = pp;
        }

        private PublicProfileParcelable(Parcel parcel)
        {
            List<PublicProfile> following = new List<PublicProfile>();
            List<PublicProfileParcelable> followingParcelable = new List<PublicProfileParcelable>();
            parcel.ReadParcelableList(followingParcelable, Java.Lang.Class.FromType(typeof(PublicProfileParcelable)).ClassLoader);
            foreach (PublicProfileParcelable p in followingParcelable)
            {
                following.Add(p.pp);
            }

            List<Podcast> subscribed = new List<Podcast>();
            List<PodcastParcelable> subscribedParcelable = new List<PodcastParcelable>();
            parcel.ReadParcelableList(subscribedParcelable, Java.Lang.Class.FromType(typeof(PodcastParcelable)).ClassLoader);
            foreach (PodcastParcelable p in subscribedParcelable)
            {
                subscribed.Add(p.podcast);
            }

            List<Post> posts = new List<Post>();
            List<PostParcelable> postsParcelable = new List<PostParcelable>();
            parcel.ReadParcelableList(postsParcelable, Java.Lang.Class.FromType(typeof(PostParcelable)).ClassLoader);
            foreach (PostParcelable p in postsParcelable)
            {
                posts.Add(p.p);
            }

            pp = new PublicProfile
            {
                Following = following,
                Subscribed = subscribed,
                Posts = posts,
                UserName = parcel.ReadString(),
                PictureLink = parcel.ReadString(),
                PublicProfileId = parcel.ReadInt()
            };
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            if(pp == null) { return; }
            if (pp.Following == null) { pp.Following = new List<PublicProfile>(); }
            if (pp.Subscribed == null) { pp.Subscribed = new List<Podcast>(); }
            if (pp.Posts == null) { pp.Posts = new List<Post>(); }

            List<PublicProfileParcelable> followingList = new List<PublicProfileParcelable>();
            foreach (PublicProfile following in pp.Following)
            {
                followingList.Add(new PublicProfileParcelable(following));
            }
            dest.WriteParcelableList(followingList, 0);

            List<PodcastParcelable> subscribedList = new List<PodcastParcelable>();
            foreach (Podcast podcast in pp.Subscribed)
            {
                subscribedList.Add(new PodcastParcelable(podcast));
            }
            dest.WriteParcelableList(subscribedList, 0);

            List<PostParcelable> postList = new List<PostParcelable>();
            foreach (Post post in pp.Posts)
            {
                postList.Add(new PostParcelable(post));
            }
            dest.WriteParcelableList(postList, 0);

            dest.WriteString(pp.UserName);
            dest.WriteString(pp.PictureLink);
            dest.WriteInt(pp.PublicProfileId);
        }

        private static readonly GenericParcelableCreator<PublicProfileParcelable> _creator
            = new GenericParcelableCreator<PublicProfileParcelable>((parcel) => new PublicProfileParcelable(parcel));

        [ExportField("CREATOR")]
        public static GenericParcelableCreator<PublicProfileParcelable> GetCreator()
        {
            return _creator;
        }

    }
}