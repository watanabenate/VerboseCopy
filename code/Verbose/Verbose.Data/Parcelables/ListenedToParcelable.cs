using Android.OS;
using Android.Runtime;
using Java.Interop;
using System;
using System.Collections.Generic;

namespace Verbose.Data
{
    public class ListenedToParcelable : Java.Lang.Object, IParcelable
    {
        public ListenedTo listenedTo { get; set; }

        public ListenedToParcelable()
        {

        }

        public ListenedToParcelable(ListenedTo l)
        {
            listenedTo = l;
        }

        private ListenedToParcelable(Parcel parcel)
        {
            listenedTo = new ListenedTo
            {
                Timestamp = parcel.ReadLong(),
                Episode = ((PodcastEpisodeParcelable)parcel.ReadParcelable(Java.Lang.Class.FromType(typeof(PodcastEpisodeParcelable)).ClassLoader)).episode,
            };
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            if(listenedTo == null) { return; }

            dest.WriteLong(listenedTo.Timestamp);
            dest.WriteParcelable(new PodcastEpisodeParcelable(listenedTo.Episode), 0);
        }

        private static readonly GenericParcelableCreator<ListenedToParcelable> _creator
            = new GenericParcelableCreator<ListenedToParcelable>((parcel) => new ListenedToParcelable(parcel));

        [ExportField("CREATOR")]
        public static GenericParcelableCreator<ListenedToParcelable> GetCreator()
        {
            return _creator;
        }

    }
}