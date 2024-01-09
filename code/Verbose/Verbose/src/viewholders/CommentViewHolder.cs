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
    internal class CommentViewHolder : RecyclerView.ViewHolder
    {
        public ImageButton ProfileImage { get; private set; }
        public TextView ProfileUsername { get; private set; }
        public TextView TimeStamp { get; private set; }
        public TextView Body { get; private set; }
        public ImageButton LikeButton { get; private set; }
        public TextView LikeCount { get; private set; }
        public ImageButton CommentButton { get; private set; }
        public TextView CommentCount { get; private set; }

        public CommentViewHolder(View itemView, Action<int> listenerLike, Action<int> listenerProfile) : base(itemView)
        {
            ProfileImage = itemView.FindViewById<ImageButton>(Resource.Id.comment_profile);
            ProfileUsername = itemView.FindViewById<TextView>(Resource.Id.comment_username);
            TimeStamp = itemView.FindViewById<TextView>(Resource.Id.comment_timestamp);
            Body = itemView.FindViewById<TextView>(Resource.Id.comment_body);
            LikeButton = itemView.FindViewById<ImageButton>(Resource.Id.comment_like_button);
            LikeCount = itemView.FindViewById<TextView>(Resource.Id.comment_like_count);

            ProfileImage.Click += (sender, e) => listenerProfile(LayoutPosition);
            ProfileUsername.Click += (sender, e) => listenerProfile(LayoutPosition);
        }
    }
}