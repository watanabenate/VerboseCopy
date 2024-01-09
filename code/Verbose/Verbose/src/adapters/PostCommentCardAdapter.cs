using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verbose.API;
using Verbose.Data;
using Verbose.src.viewholders;

namespace Verbose.src.adapters
{
    internal class PostCommentCardAdapter : RecyclerView.Adapter
    {
        public override int ItemCount => commentList.Count;

        public List<Comment> commentList;
        VerboseAPIService _api;

        public PostCommentCardAdapter(List<Comment> commentList)
        {
            this.commentList = commentList;
            _api = VerboseAPIService.Instance;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.post_comment_card, parent, false);

            return new CommentViewHolder(itemView, LikeClick, ProfileClick);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            CommentViewHolder vh = holder as CommentViewHolder;
            Bitmap b = _api.GetImageBitmap(commentList[position].ProfileImageLink);
            vh.ProfileImage.SetImageBitmap(b);
            vh.ProfileUsername.Text = commentList[position].Username;
            // TODO: adjust timestamp to use actual date/time posted
            vh.TimeStamp.Text = _api.getTimeString(commentList[position].Timestamp);
            vh.Body.Text = commentList[position].Text;
            vh.LikeCount.Text = commentList[position].Likes.ToString();
        }

        private void LikeClick(int position)
        {
            if (LikeClickHandler != null)
                LikeClickHandler(this, position);
        }

        public event EventHandler<int> LikeClickHandler;

        private void ProfileClick(int position)
        {
            if (ProfileClickHandler != null)
                ProfileClickHandler(this, position);
        }

        public event EventHandler<int> ProfileClickHandler;
    }
}

