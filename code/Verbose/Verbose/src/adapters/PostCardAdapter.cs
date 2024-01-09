using Android.App;
using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using Verbose.API;
using Verbose.Data;
using Verbose.src.viewholders;

namespace Verbose.src.adapters
{
    public class PostCardAdapter : RecyclerView.Adapter
    {
        public override int ItemCount => postList.Count;

        public List<Post> postList;
        VerboseAPIService _api;

        public PostCardAdapter()
        {
            postList = new List<Post>();
            // Get the api
            _api = VerboseAPIService.Instance;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.post_card, parent, false);

            return new PostViewHolder(itemView, OnLikeClick, OnPostClick, OnProfileClick, OnEpisodeClick);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            PostViewHolder vh = holder as PostViewHolder;

            if (postList[position].Comments == null) { postList[position].Comments = new List<Comment>(); }
            if (postList[position].LikedBy == null) { postList[position].LikedBy = new HashSet<LikedBy>(); }

            if (postList[position].ProfileImageLink != null)
            {
                vh.ProfileImage.SetImageBitmap(_api.GetImageBitmap(postList[position].ProfileImageLink));
            }
            else
            {
                vh.ProfileImage.SetImageBitmap(BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Mipmap.profile_temp_fill));
            }
            if(postList[position].ImageURL != null && postList[position].ImageURL != "")
            {
                vh.PodcastImage.Visibility = ViewStates.Visible;
                vh.PodcastImage.SetImageBitmap(_api.GetImageBitmap(postList[position].ImageURL));
            }
            else
            {
                vh.PodcastImage.Visibility = ViewStates.Gone;
            }
            
            vh.Username.Text = postList[position].Username;
            vh.PostTitle.Text = postList[position].Title;
            vh.PostBody.Text = postList[position].Description;
            vh.LikeCount.Text = Math.Max(0, postList[position].LikedBy.Count).ToString();
            vh.CommentCount.Text = postList[position].Comments.Count.ToString();

            LikedBy tempLikedBy = new LikedBy
            {
                PostID = postList[position].PostID,
                PublicProfileID = _api.UserProfile.PublicProfileInfo.PublicProfileId
            };

            if (postList[position].LikedBy.Contains(tempLikedBy))
            {
                vh.LikeButton.SetImageResource(Resource.Drawable.filled_like);
                vh.isLiked = true;
            }
            
        }

        async void OnLikeClick(Tuple<int, bool> tuple)
        {
            // Item1 = index, Item2 = isLiked
            Post sender = postList[tuple.Item1];
            LikedBy likedBy = new LikedBy
            {
                PostID = sender.PostID,
                PublicProfileID = _api.UserProfile.PublicProfileInfo.PublicProfileId
            };
            if(tuple.Item2)
            {
                sender.LikedBy.Add(likedBy);
                sender.Likes += 1;
            }
            else
            {
                sender.LikedBy.Remove(likedBy);
                sender.Likes -= 1;
            }

            await _api.LikeOrUnlikePost(sender, tuple.Item2);
            
        }

        public event EventHandler<int> PostClick;
        void OnPostClick(int position)
        {
            if (PostClick != null)
                PostClick(this, position);
        }

        public event EventHandler<int> ProfileClick;
        void OnProfileClick(int position)
        {
            if(ProfileClick != null)
                ProfileClick(this, position);
        }

        public event EventHandler<int> EpisodeClick;
        void OnEpisodeClick(int position) 
        {
            if(EpisodeClick != null)
                EpisodeClick(this, position);
        }
    }
}