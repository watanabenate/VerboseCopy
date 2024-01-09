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
    internal class PostViewHolder : RecyclerView.ViewHolder
    {
        public ImageButton ProfileImage { get; private set; }
        public TextView Username { get; private set; }
        public TextView PostTitle { get; private set; }
        public TextView PostBody { get; private set; }
        public TextView LikeCount { get; set; }
        public ImageButton LikeButton { get; private set; }
        public TextView CommentCount { get; set; }
        public ImageButton CommentButton { get; private set; }
        public ImageView PodcastImage { get; private set; }
        public bool isLiked = false;

        Action<Tuple<int, bool>> l;

        public PostViewHolder(View itemView, Action<Tuple<int, bool>> likeListener, Action<int> postListener, Action<int> profileListener, Action<int> episodeListener) : base(itemView)
        {
            // Locate and cache view references:
            ProfileImage = itemView.FindViewById<ImageButton>(Resource.Id.profile_image);
            Username = itemView.FindViewById<TextView>(Resource.Id.username);
            PostTitle = itemView.FindViewById<TextView>(Resource.Id.post_title);
            PostBody = itemView.FindViewById<TextView>(Resource.Id.post_content);
            LikeCount = itemView.FindViewById<TextView>(Resource.Id.like_count);
            LikeButton = itemView.FindViewById<ImageButton>(Resource.Id.like);
            CommentCount = itemView.FindViewById<TextView>(Resource.Id.comment_count);
            CommentButton = itemView.FindViewById<ImageButton>(Resource.Id.comment);
            PodcastImage = itemView.FindViewById<ImageView>(Resource.Id.podcast_image);

            l = likeListener;

            LikeButton.Click += LikeClick;

            ProfileImage.Click += (s, e) => profileListener(base.LayoutPosition);
            Username.Click += (s, e) => profileListener(base.LayoutPosition); ;

            PodcastImage.Click += (s, e) => episodeListener(base.LayoutPosition); ;

            itemView.Click += (s, e) => postListener(base.LayoutPosition);
        }

        private void LikeClick(object sender, EventArgs e)
        {
            if(isLiked)
            {
                LikeButton.SetImageResource(Resource.Drawable.empty_like);
                LikeCount.Text = (int.Parse(LikeCount.Text) - 1).ToString();
                isLiked = false;
            }
            else
            {
                LikeButton.SetImageResource(Resource.Drawable.filled_like);
                LikeCount.Text = (int.Parse(LikeCount.Text) + 1).ToString();
                isLiked = true;
            }

            l(new Tuple<int, bool>(LayoutPosition, isLiked));
        }
    }
}