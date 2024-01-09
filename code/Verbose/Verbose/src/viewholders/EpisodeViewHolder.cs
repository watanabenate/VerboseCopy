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
    internal class EpisodeViewHolder : RecyclerView.ViewHolder
    {
        public ImageButton CoverArt { get; private set; }
        public TextView EpisodeTitle { get; private set; }

        public EpisodeViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            CoverArt = itemView.FindViewById<ImageButton>(Resource.Id.cover_art);
            EpisodeTitle = itemView.FindViewById<TextView>(Resource.Id.episode_title);

            CoverArt.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}