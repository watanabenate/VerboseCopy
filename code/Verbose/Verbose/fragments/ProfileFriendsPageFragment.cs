using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Verbose.API;
using Verbose.Data;
using Verbose.src.adapters;

namespace Verbose
{
    public class ProfileFriendsPageFragment : AndroidX.Fragment.App.Fragment
    {
        VerboseAPIService _api;

        RecyclerView friendsListRecycler;

        RecyclerView.LayoutManager mLayoutManager;

        FriendCardAdapter mFriendAdapter;

        View view;
        ImageButton settingsBtn;
        Button subBtn;
        TextView usernameText;
        ImageView profilePhoto;
        TextView subsCountText;
        TextView friendsCountText;

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
            view = inflater.Inflate(Resource.Layout.user_friends_page, container, false);

            base.OnCreate(savedInstanceState);

            // Get the api
            _api = VerboseAPIService.Instance;

            SetupPage();

            friendsListRecycler = view.FindViewById<RecyclerView>(Resource.Id.friend_list_recycler);
            mLayoutManager = new LinearLayoutManager(Context);
            friendsListRecycler.SetLayoutManager(mLayoutManager);

            mFriendAdapter = new FriendCardAdapter();
            mFriendAdapter.friendList = _api.UserProfile.PublicProfileInfo.Following;
            friendsListRecycler.SetAdapter(mFriendAdapter);

            friendsPageCountText = view.FindViewById<TextView>(Resource.Id.friends_page_count);
            friendsPageCountText.Text = _api.UserProfile.PublicProfileInfo.Following.Count.ToString();

            mFriendAdapter.UnfollowProfileClick += UnfollowProfile;
            mFriendAdapter.ProfileClick += GoToProfile;

            return view;
        }

        private void SetupPage()
        {
            subBtn = view.FindViewById<Button>(Resource.Id.subs);
            subBtn.Click += SubClick;

            settingsBtn = view.FindViewById<ImageButton>(Resource.Id.settings_btn);
            settingsBtn.Click += SettingsClick;

            usernameText = view.FindViewById<TextView>(Resource.Id.user);
            usernameText.Text = _api.UserProfile.PublicProfileInfo.UserName;

            profilePhoto = view.FindViewById<ImageView>(Resource.Id.small_profile);
            if(_api.UserProfile.PublicProfileInfo.PictureLink != "" && _api.UserProfile.PublicProfileInfo.PictureLink != null)
            {
                profilePhoto.SetImageBitmap(_api.GetImageBitmap(_api.UserProfile.PublicProfileInfo.PictureLink));
            }

            subsCountText = view.FindViewById<TextView>(Resource.Id.subs_count);
            subsCountText.Text = _api.UserProfile.PublicProfileInfo?.Subscribed.Count.ToString();

            friendsCountText = view.FindViewById<TextView>(Resource.Id.friends_count);
            friendsCountText.Text = _api.UserProfile.PublicProfileInfo?.Following.Count.ToString();
        }

        private void SettingsClick(object sender, EventArgs e)
        {
            SettingsPageFragment settingsFragment = new SettingsPageFragment();

            // Send the url of the podcast
            ((MainPageActivity)Activity).ChangeFragment(settingsFragment, Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_slide_out_bottom);
        }

        private void SubClick(object sender, EventArgs e)
        {
            ProfileSubsPageFragment subsFragment = new ProfileSubsPageFragment();

            // Send the url of the podcast
            ((MainPageActivity)Activity).ChangeFragment(subsFragment);
        }

        private async void UnfollowProfile(object sender, int position)
        {
            FriendCardAdapter adapter = (FriendCardAdapter)sender;
            PublicProfile pp = adapter.friendList[position];

            if (await _api.FollowOrUnfollowProfile(pp, false))
            {
                mFriendAdapter.NotifyDataSetChanged();

                Toast.MakeText(Context, "Unfollowed " + pp.UserName, ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(Context, "Could not unfollow " + pp.UserName, ToastLength.Short).Show();
            }

            friendsCountText.Text = _api.UserProfile.PublicProfileInfo?.Following.Count.ToString();
            friendsPageCountText.Text = _api.UserProfile.PublicProfileInfo.Following.Count.ToString();
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