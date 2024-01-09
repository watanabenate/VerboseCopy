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
    public class ProfileSubsPageFragment : AndroidX.Fragment.App.Fragment
    {
        VerboseAPIService _api;

        RecyclerView subsListRecycler;

        RecyclerView.LayoutManager mLayoutManager;

        SubscribedCardAdapter mSubAdapter;

        View view;
        ImageButton settingsBtn;
        Button friendBtn;
        TextView usernameText;
        ImageView profilePhoto;
        TextView subsCountText;
        TextView friendsCountText;

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
            view = inflater.Inflate(Resource.Layout.user_subs_page, container, false);

            base.OnCreate(savedInstanceState);

            // Get the api
            _api = VerboseAPIService.Instance;

            SetupPage();

            subsListRecycler = view.FindViewById<RecyclerView>(Resource.Id.sub_list_recycler);
            mLayoutManager = new LinearLayoutManager(Context);
            subsListRecycler.SetLayoutManager(mLayoutManager);

            mSubAdapter = new SubscribedCardAdapter();
            mSubAdapter.subscribedPodcastList = _api.UserProfile.PublicProfileInfo.Subscribed;
            mSubAdapter.UnsubscribeSubscribeClick += UnsubscribeFromPodcast;
            mSubAdapter.PodcastClick += GoToPodcast;
            subsListRecycler.SetAdapter(mSubAdapter);

            subsPageCountText = view.FindViewById<TextView>(Resource.Id.subs_page_count);
            subsPageCountText.Text = _api.UserProfile.PublicProfileInfo.Subscribed.Count.ToString();

            return view;
        }

        private void SetupPage()
        {
            friendBtn = view.FindViewById<Button>(Resource.Id.friends);
            friendBtn.Click += FriendClick;

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

        private void FriendClick(object sender, EventArgs e)
        {
            ProfileFriendsPageFragment friendsFragment = new ProfileFriendsPageFragment();

            // Send the url of the podcast
            ((MainPageActivity)Activity).ChangeFragment(friendsFragment);
        }

        private async void UnsubscribeFromPodcast(object sender, int position)
        {
            if(unsubscribeActive)
            {
                return;
            }

            unsubscribeActive = true;
            SubscribedCardAdapter adapter = (SubscribedCardAdapter)sender;
            Podcast podcast = adapter.subscribedPodcastList[position];

            if(await _api.SubOrUnsubPodcast(false, podcast))
            {
                adapter.NotifyDataSetChanged();
                subsCountText.Text = adapter.ItemCount.ToString();
                subsPageCountText.Text =adapter.ItemCount.ToString();
            }
            else
            {
                Toast.MakeText(Context, "Could not remove this subscribed podcast", ToastLength.Short).Show(); 
            }
            unsubscribeActive = false;
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
    }
}