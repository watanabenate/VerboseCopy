using Android.OS;
using Android.Runtime;
using Java.Interop;
using System.Collections.Generic;

namespace Verbose.Data
{
    public class PodcastParcelable : Java.Lang.Object, IParcelable
    {
        public Podcast podcast { get; set; }

        public PodcastParcelable()
        {

        }

        public PodcastParcelable(Podcast p)
        {
            podcast = p;
        }

        private PodcastParcelable(Parcel parcel)
        {
            // Read in our list of episodes first
            List<PodcastEpisode> episodes = new List<PodcastEpisode>();
            List<PodcastEpisodeParcelable> episodesParcelable = new List<PodcastEpisodeParcelable>();
            parcel.ReadParcelableList(episodesParcelable, Java.Lang.Class.FromType(typeof(PodcastEpisodeParcelable)).ClassLoader);
            foreach (PodcastEpisodeParcelable e in episodesParcelable)
            {
                episodes.Add(e.episode);
            }

            // Make the podcast
            podcast = new Podcast
            {
                Creator = parcel.ReadString(),
                CoverArtLink = parcel.ReadString(),
                Title = parcel.ReadString(),
                Description = parcel.ReadString(),
                Episodes = episodes
            };
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            if (podcast == null) { return; }
            if (podcast.Episodes == null) { podcast.Episodes = new List<PodcastEpisode> (); }

            List<PodcastEpisodeParcelable> episodes = new List<PodcastEpisodeParcelable>();
            foreach (PodcastEpisode e in podcast.Episodes)
            {
                episodes.Add(new PodcastEpisodeParcelable(e));
            }
            dest.WriteParcelableList(episodes, 0);

            dest.WriteString(podcast.Creator);
            dest.WriteString(podcast.CoverArtLink);
            dest.WriteString(podcast.Title);
            dest.WriteString(podcast.Description);
        }

        private static readonly GenericParcelableCreator<PodcastParcelable> _creator
            = new GenericParcelableCreator<PodcastParcelable>((parcel) => new PodcastParcelable(parcel));

        [ExportField("CREATOR")]
        public static GenericParcelableCreator<PodcastParcelable> GetCreator()
        {
            return _creator;
        }

    }
}