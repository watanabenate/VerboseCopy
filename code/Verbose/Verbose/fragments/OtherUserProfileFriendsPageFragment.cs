using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content.Resources;
using AndroidX.RecyclerView.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Verbose.API;
using Verbose.Data;
using Verbose.src.adapters;

namespace Verbose
{
    public class OtherUserProfileFriendsPageFragment : AndroidX.Fragment.App.Fragment
    {
        VerboseAPIService _api;

        RecyclerView friendsListRecycler;

        RecyclerView.LayoutManager mLayoutManager;

        FriendCardAdapter mFriendAdapter;

        View view;
        Button subBtn;
        TextView usernameText;
        ImageView profilePhoto;
        TextView subsCountText;
        TextView friendsCountText;
        ImageButton addFriendBtn;

        TextView friendsPageCountText;

        /// <summary>
        /// This is called whenever the fragment is made.
        /// </summary>
        /// <param name="inflater"></param>
        /// <param name="container"></param>
        /// <param name="savedInstanceState"></param>
        /// <returns></returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            container.RemoveAllViews();
            view = inflater.Inflate(Resource.Layout.other_user_friends_page, container, false);

            base.OnCreate(savedInstanceState);

            // Get the api
            _api = VerboseAPIService.Instance;

            SetupPage();

            friendsListRecycler = view.FindViewById<RecyclerView>(Resource.Id.friend_list_recycler);
            mLayoutManager = new LinearLayoutManager(Context);
            friendsListRecycler.SetLayoutManager(mLayoutManager);

            mFriendAdapter = new FriendCardAdapter(true);
            mFriendAdapter.friendList = _api.OtherUserProfile.Following;
            friendsListRecycler.SetAdapter(mFriendAdapter);
            mFriendAdapter.ProfileClick += GoToProfile;

            friendsPageCountText = view.FindViewById<TextView>(Resource.Id.friends_page_count);
            friendsPageCountText.Text = _api.OtherUserProfile.Following.Count.ToString();

            return view;
        }

        private void SetupPage()
        {
            subBtn = view.FindViewById<Button>(Resource.Id.subs);
            subBtn.Click += SubClick;

            usernameText = view.FindViewById<TextView>(Resource.Id.user);
            usernameText.Text = _api.OtherUserProfile.UserName;

            profilePhoto = view.FindViewById<ImageView>(Resource.Id.small_profile);
            if(_api.OtherUserProfile.PictureLink != "" && _api.OtherUserProfile.PictureLink != null)
            {
                profilePhoto.SetImageBitmap(_api.GetImageBitmap(_api.OtherUserProfile.PictureLink));
            }

            subsCountText = view.FindViewById<TextView>(Resource.Id.subs_count);
            subsCountText.Text = _api.OtherUserProfile.Subscribed.Count.ToString();

            friendsCountText = view.FindViewById<TextView>(Resource.Id.friends_count);
            friendsCountText.Text = _api.OtherUserProfile.Following.Count.ToString();

            addFriendBtn = view.FindViewById<ImageButton>(Resource.Id.follow_unfollow_btn);
            int addFriendIcon;

            if (_api.OtherUserInUserFollowing)
                addFriendIcon = Resource.Drawable.checkmark;
            else
                addFriendIcon = Resource.Drawable.add_friend;

            addFriendBtn.SetImageResource(addFriendIcon);
            addFriendBtn.Click += FriendUnfriendProfile;
        }

        private void SubClick(object sender, EventArgs e)
        {
            OtherUserProfileSubsPageFragment subsFragment = new OtherUserProfileSubsPageFragment();

            // Send the url of the podcast
            ((MainPageActivity)Activity).ChangeFragment(subsFragment);
        }

        private async void FriendUnfriendProfile(object sender, EventArgs e)
        {
            if (await _api.FollowOrUnfollowProfile(_api.OtherUserProfile, !_api.OtherUserInUserFollowing))
            {
                string followOrUnfollowed = _api.OtherUserInUserFollowing ? "unfollowed" : "followed";
                Toast.MakeText(Context, "Successfully " + followOrUnfollowed + " " + _api.OtherUserProfile.UserName, ToastLength.Short).Show();

                // Switch unfollow/follow so we know which one we are
                _api.OtherUserInUserFollowing = !_api.OtherUserInUserFollowing;

                if (_api.OtherUserInUserFollowing)
                {
                    addFriendBtn.SetImageResource(Resource.Drawable.checkmark);
                }
                else
                {
                    addFriendBtn.SetImageResource(Resource.Drawable.add_friend);
                }
            }
        }

        private async void GoToProfile(object sender, int position)
        {
            FriendCardAdapter adapter = (FriendCardAdapter)sender;
            PublicProfile p = adapter.friendList[position];

            // Make sure to have a check if it's the same user or a different user
            // Don't want to navigate to other profile page if it's the current user
            if (p.UserName == _api.UserProfile.PublicProfileInfo.UserName)
            {
                ProfilePageFragment profileFragment = new ProfilePageFragment();
                ((MainPageActivity)Activity).ChangeFragment(profileFragment);
            }
            else
            {
                // Get the other user first
                if (await _api.GetOtherUserPublicProfile(p.UserName))
                {
                    OtherUserProfilePageFragment otherProfileFragment = new OtherUserProfilePageFragment();
                    ((MainPageActivity)Activity).ChangeFragment(otherProfileFragment);
                }
                else
                {
                    Toast.MakeText(Context, "Could not load other user's profile page", ToastLength.Short).Show();
                }
            }
        }
    }
}