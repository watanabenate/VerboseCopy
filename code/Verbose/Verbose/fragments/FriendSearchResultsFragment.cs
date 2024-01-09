using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Java.Interop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Verbose.API;
using Verbose.Data;
using Verbose.fragments;
using Verbose.src.adapters;

namespace Verbose
{
    public class FriendSearchResultsFragment : AndroidX.Fragment.App.Fragment
    {
        VerboseAPIService _api;

        RecyclerView.LayoutManager mLayoutManager;

        private bool searchActive = false;

        View view;

        TextView noResultsFoundText;

        SearchView friendSearchBar;
        RecyclerView friendSearchResultRecycler;
        FriendCardAdapter friendSearchResultAdapter;
        ProgressBar progressSpinner;

        /// <summary>
        /// This is called whenever the fragment is made.
        /// 
        /// This page has two views: Search and SearchResults
        /// </summary>
        /// <param name="inflater"></param>
        /// <param name="container"></param>
        /// <param name="savedInstanceState"></param>
        /// <returns></returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            container.RemoveAllViews();
            view = inflater.Inflate(Resource.Layout.friend_search_results_page, container, false);

            base.OnCreate(savedInstanceState);

            progressSpinner = view.FindViewById<ProgressBar>(Resource.Id.feed_progress_bar);
            progressSpinner.Visibility = ViewStates.Gone;

            savedInstanceState = this.Arguments;


            // Get the api
            _api = VerboseAPIService.Instance;

            friendSearchBar = view.FindViewById<SearchView>(Resource.Id.friend_search_bar);
            friendSearchBar.QueryTextSubmit += FriendSearchBarSubmit;
            string temp = savedInstanceState.GetString("search");
            friendSearchBar.SetQuery(temp, false);

            progressSpinner = view.FindViewById<ProgressBar>(Resource.Id.feed_progress_bar);

            friendSearchBar = view.FindViewById<SearchView>(Resource.Id.friend_search_bar);
            friendSearchBar.QueryTextSubmit += FriendSearchBarSubmit;

            friendSearchResultRecycler = view.FindViewById<RecyclerView>(Resource.Id.friend_results_recycler);
            mLayoutManager = new LinearLayoutManager(Context);
            friendSearchResultRecycler.SetLayoutManager(mLayoutManager);

            friendSearchResultAdapter = new FriendCardAdapter(true);
            friendSearchResultAdapter.UnfollowProfileClick += FriendUnfriendProfile;
            friendSearchResultAdapter.ProfileClick += GoToProfile;
            friendSearchResultAdapter.friendList = _api.friendSearchResults;

            friendSearchResultRecycler.SetAdapter(friendSearchResultAdapter);

            noResultsFoundText = view.FindViewById<TextView>(Resource.Id.friend_no_results);
            if(friendSearchResultAdapter.friendList.Count == 0)
            {
                noResultsFoundText.Visibility = ViewStates.Visible;
            }
            else
            {
                noResultsFoundText.Visibility = ViewStates.Gone;
            }

            return view;
        }

        private async void FriendSearchBarSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            if (!searchActive)
            {
                searchActive = true;
                progressSpinner.Visibility = ViewStates.Visible;
                noResultsFoundText.Visibility = ViewStates.Gone;

                SearchView sv = sender as SearchView;
                string text = sv.Query;

                if (await Task.Run(() => { return _api.SearchUsers(text); }))
                {
                    friendSearchResultAdapter.friendList = _api.friendSearchResults;
                    friendSearchResultAdapter.NotifyDataSetChanged();

                    if (friendSearchResultAdapter.friendList.Count == 0)
                    {
                        noResultsFoundText.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        noResultsFoundText.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    Toast.MakeText(Context, "Search Failed on the Verbose Server", ToastLength.Short).Show();
                }

                progressSpinner.Visibility = ViewStates.Gone;
                searchActive = false;
            }
        }

        private async void GoToProfile(object sender, int position)
        {
            progressSpinner.Visibility = ViewStates.Visible;

            FriendCardAdapter adapter = (FriendCardAdapter)sender;
            PublicProfile profile = adapter.friendList[position];

            // Make sure to have a check if it's the same user or a different user
            // Don't want to navigate to other profile page if it's the current user
            if (profile.UserName == _api.UserProfile.PublicProfileInfo.UserName)
            {
                ProfilePageFragment profileFragment = new ProfilePageFragment();
                ((MainPageActivity)Activity).ChangeFragment(profileFragment);
            }
            else
            {
                // Get the other user first
                if (await _api.GetOtherUserPublicProfile(profile.UserName))
                {
                    OtherUserProfilePageFragment otherProfileFragment = new OtherUserProfilePageFragment();
                    ((MainPageActivity)Activity).ChangeFragment(otherProfileFragment);
                }
                else
                {
                    Toast.MakeText(Context, "Could not load other user's profile page", ToastLength.Short).Show();
                    progressSpinner.Visibility = ViewStates.Gone;
                }
            }

        }

        private async void FriendUnfriendProfile(object sender, int position)
        {
            FriendCardAdapter adapter = (FriendCardAdapter)sender;
            PublicProfile pp = adapter.friendList[position];

            bool followOrUnfollow = !_api.UserProfile.PublicProfileInfo.Following.Contains(pp); // We want to either unfriend or friend them

            if (await _api.FollowOrUnfollowProfile(pp, followOrUnfollow))
            {
                friendSearchResultAdapter.NotifyDataSetChanged();

                string followOrUnfollowed = followOrUnfollow ? "followed" : "unfollowed";
                Toast.MakeText(Context, "Successfully " + followOrUnfollowed + " " + pp.UserName, ToastLength.Short).Show();
            }
        }
    }
}
