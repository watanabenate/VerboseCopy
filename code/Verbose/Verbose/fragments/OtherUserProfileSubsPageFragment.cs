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
using Verbose.fragments;
using Verbose.src.adapters;

namespace Verbose
{
    public class OtherUserProfileSubsPageFragment : AndroidX.Fragment.App.Fragment
    {
        VerboseAPIService _api;

        RecyclerView subsListRecycler;

        RecyclerView.LayoutManager mLayoutManager;

        SubscribedCardAdapter mSubAdapter;

        View view;
        Button friendBtn;
        TextView usernameText;
        ImageView profilePhoto;
        TextView subsCountText;
        TextView friendsCountText;
        ImageButton addFriendBtn;

        TextView subsPageCountText;

        private bool unsubscribeActive = false;

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
            view = inflater.Inflate(Resource.Layout.other_user_subs_page, container, false);

            base.OnCreate(savedInstanceState);

            // Get the api
            _api = VerboseAPIService.Instance;

            SetupPage();

            subsListRecycler = view.FindViewById<RecyclerView>(Resource.Id.sub_list_recycler);
            mLayoutManager = new LinearLayoutManager(Context);
            subsListRecycler.SetLayoutManager(mLayoutManager);

            mSubAdapter = new SubscribedCardAdapter(true);
            mSubAdapter.subscribedPodcastList = _api.OtherUserProfile.Subscribed;
            mSubAdapter.PodcastClick += GoToPodcast;
            mSubAdapter.ShowUnsubscribeButton = false;
            subsListRecycler.SetAdapter(mSubAdapter);

            subsPageCountText = view.FindViewById<TextView>(Resource.Id.subs_page_count);
            subsPageCountText.Text = _api.OtherUserProfile.Subscribed.Count.ToString();

            return view;
        }

        private void SetupPage()
        {
            friendBtn = view.FindViewById<Button>(Resource.Id.friends);
            friendBtn.Click += FriendClick;

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

        private void FriendClick(object sender, EventArgs e)
        {
            OtherUserProfileFriendsPageFragment friendsFragment = new OtherUserProfileFriendsPageFragment();

            // Send the url of the podcast
            ((MainPageActivity)Activity).ChangeFragment(friendsFragment);
        }

        private async void GoToPodcast(object sender, int position)
        {
            SubscribedCardAdapter adapter = (SubscribedCardAdapter)sender;
            Podcast adapterPodcast = adapter.subscribedPodcastList[position];

            if (await _api.GetPodcastFromId(adapterPodcast.PodcastID))
            {
                Podcast p = _api.PodcastFromId;

                Bundle b = new Bundle();
                b.PutParcelable("Podcast", new PodcastParcelable(p));

                // Get the other user first
                PodcastPageFragment podcastFragment = new PodcastPageFragment();
                podcastFragment.Arguments = b;
                ((MainPageActivity)Activity).ChangeFragment(podcastFragment);
            }
            else
            {
                Toast.MakeText(Context, "Could not load podcast", ToastLength.Short).Show();
            }
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
    }
}