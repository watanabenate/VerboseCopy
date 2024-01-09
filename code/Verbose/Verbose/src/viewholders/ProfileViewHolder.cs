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
    internal class ProfileViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Username { get; private set; }
        public Button Button { get; private set; }

        public ProfileViewHolder(View itemView) : base(itemView)
        {
            // Locate and cache view references:
            Image = itemView.FindViewById<ImageView>(Resource.Id.profile_image);
            Username = itemView.FindViewById<TextView>(Resource.Id.profile_name);
            Button = itemView.FindViewById<Button>(Resource.Id.friend_button);
        }
    }
}