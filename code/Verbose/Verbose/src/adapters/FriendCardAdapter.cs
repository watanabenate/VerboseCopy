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
using Verbose.Data;
using Verbose.API;
using Verbose.src.viewholders;
using Android.Graphics;

namespace Verbose.src.adapters
{
    public class FriendCardAdapter : RecyclerView.Adapter
    {
        public override int ItemCount => friendList.Count;

        public List<PublicProfile> friendList;

        private VerboseAPIService _api;

        public bool OnOtherUserPage;

        // TODO: create constructor that takes some sort of List depending on structures used for models
        public FriendCardAdapter()
        {
            friendList = new List<PublicProfile>();
            _api = VerboseAPIService.Instance;
            OnOtherUserPage = false;
        }

        public FriendCardAdapter(bool OnOtherUserPage) : this()
        {
            this.OnOtherUserPage = OnOtherUserPage;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.friend_card, parent, false);

            return new FriendViewHolder(itemView, OnUnfriendFriendClick, OnProfileClick);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            FriendViewHolder vh = holder as FriendViewHolder;

            if(friendList[position].PictureLink != null && friendList[position].PictureLink != "")
            {
                vh.profileImage.SetImageBitmap(_api.GetImageBitmap(friendList[position].PictureLink));
            }
            else
            {
                vh.profileImage.SetImageBitmap(BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Mipmap.profile_temp_fill));
            }

            if(friendList[position].UserName == _api.UserProfile.PublicProfileInfo.UserName)
            {
                vh.unfriendFriendButton.Visibility = ViewStates.Invisible;
            }
            else
            {
                vh.unfriendFriendButton.Visibility = ViewStates.Visible;
            }

            if (!OnOtherUserPage)
            {
                vh.unfriendFriendButton.Visibility = ViewStates.Visible;
                vh.unfriendFriendButton.Text = "Unfriend";
            }
            else
            {
                vh.unfriendFriendButton.Visibility = ViewStates.Invisible;
            }

            vh.profileUsername.Text = friendList[position].UserName;
        }

        public event EventHandler<int> UnfollowProfileClick;
        void OnUnfriendFriendClick(int position)
        {
            if (UnfollowProfileClick != null)
                UnfollowProfileClick(this, position);
        }

        public event EventHandler<int> ProfileClick;
        void OnProfileClick(int position)
        {
            if(ProfileClick != null)
                ProfileClick(this, position);
        }
    }
}