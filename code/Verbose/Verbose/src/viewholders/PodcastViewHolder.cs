using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Verbose.src.viewholders
{
    internal class PodcastViewHolder : RecyclerView.ViewHolder
    {
        public ImageView CoverArt { get; private set; }
        public TextView PodcastTitle { get; private set; }
        public TextView PodcastDescription { get; private set; }
        public ImageView PlayIcon { get; private set; }

        public PodcastViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            CoverArt = itemView.FindViewById<ImageView>(Resource.Id.rv_episode_image);
            PodcastTitle = itemView.FindViewById<TextView>(Resource.Id.rv_episode_title);

            RelativeLayout.LayoutParams layoutParams =
                (RelativeLayout.LayoutParams)PodcastTitle.LayoutParameters;
            layoutParams.AddRule(LayoutRules.CenterInParent);
            PodcastTitle.LayoutParameters = layoutParams;

            PodcastDescription = itemView.FindViewById<TextView>(Resource.Id.rv_episode_desc);
            PodcastDescription.Visibility = ViewStates.Gone;

            PlayIcon = itemView.FindViewById<ImageView>(Resource.Id.episode_play_btn);

            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}