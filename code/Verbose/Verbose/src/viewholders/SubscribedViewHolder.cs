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
    internal class SubscribedViewHolder : RecyclerView.ViewHolder
    {
        public ImageView podcastImage { get; private set; }
        public TextView podcastName { get; private set; }
        public Button unsubscribeSubscribeButton { get; private set; }
        public SubscribedViewHolder(View itemView, Action<int> unsubscribeSubscribeListener, Action<int> podcastListener) : base(itemView)
        {
            podcastImage = itemView.FindViewById<ImageView>(Resource.Id.podcast_image);
            podcastName = itemView.FindViewById<TextView>(Resource.Id.podcast_name);
            unsubscribeSubscribeButton = itemView.FindViewById<Button>(Resource.Id.subscribe_button);
            unsubscribeSubscribeButton.Click += (sender, e) => unsubscribeSubscribeListener(LayoutPosition);
            itemView.Click += (sender, e) => podcastListener(LayoutPosition);
        }
    }
}