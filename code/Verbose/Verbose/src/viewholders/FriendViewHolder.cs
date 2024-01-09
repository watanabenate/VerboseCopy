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
    internal class FriendViewHolder : RecyclerView.ViewHolder
    {
        public ImageView profileImage { get; private set; }
        public TextView profileUsername { get; private set; }
        public Button unfriendFriendButton { get; private set; }
        public FriendViewHolder(View itemView, Action<int> unfriendFriendListener, Action<int> profileListener) : base(itemView)
        {
            profileImage = itemView.FindViewById<ImageView>(Resource.Id.profile_image);
            profileUsername = itemView.FindViewById<TextView>(Resource.Id.profile_name);
            unfriendFriendButton = itemView.FindViewById<Button>(Resource.Id.friend_button);
            unfriendFriendButton.Click += (sender, e) => unfriendFriendListener(LayoutPosition);
            itemView.Click += (sender, e) => profileListener(LayoutPosition);
        }
    }
}