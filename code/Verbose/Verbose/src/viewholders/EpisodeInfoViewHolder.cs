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
    internal class EpisodeInfoViewHolder : RecyclerView.ViewHolder
    {
        public ImageView CoverArt { get; private set; }
        public TextView Title { get; private set; }
        public TextView Description { get; private set; }

        // public TextView Length { get; private set; }

        public EpisodeInfoViewHolder(View itemView, Action<int> clickListener) : base(itemView)
        {
            CoverArt = itemView.FindViewById<ImageView>(Resource.Id.rv_episode_image);
            Title = itemView.FindViewById<TextView>(Resource.Id.rv_episode_title);
            Description = itemView.FindViewById<TextView>(Resource.Id.rv_episode_desc);
            // Length = itemView.FindViewById<TextView>(Resource.Id.rv_episode_len);

            itemView.Click += (sender, e) => clickListener(LayoutPosition);
        }

    }
}