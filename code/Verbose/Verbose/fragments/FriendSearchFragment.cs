using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verbose.API;

namespace Verbose
{
    public class FriendSearchFragment : AndroidX.Fragment.App.Fragment
    {
        VerboseAPIService _api;

        private bool searchActive = false;

        View view;

        SearchView searchBar;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            container.RemoveAllViews();
            view = inflater.Inflate(Resource.Layout.friend_search_page, container, false);

            base.OnCreate(savedInstanceState);

            // Get the api
            _api = VerboseAPIService.Instance;

            // Set up search functionality
            searchBar = view.FindViewById<SearchView>(Resource.Id.friend_search_bar);
            searchBar.QueryTextSubmit += FriendSearchBarSubmit;

            return view;
        }

        private async void FriendSearchBarSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            if (!searchActive)
            {
                searchActive = true;

                SearchView sv = sender as SearchView;
                string text = sv.Query;

                if (await Task.Run(() => { return _api.SearchUsers(text); })) // Run this on a separate thread so we can still interact with the app
                {
                    SetFriendSearchResultsPage();
                }
                else
                {
                    Toast.MakeText(Context, "Search Failed on the Verbose Server", ToastLength.Short).Show();
                }

                searchActive = false;
            }
        }

        private void SetFriendSearchResultsPage()
        {
            FriendSearchResultsFragment resultFragment = new FriendSearchResultsFragment();

            Bundle bundle = new Bundle();
            // TODO: change bundle name?
            bundle.PutString("search", searchBar.Query);
            resultFragment.Arguments = bundle;

            // Send the url of the podcast
            ((MainPageActivity)Activity).ChangeFragment(resultFragment);
        }
    }
}