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
using Verbose.API;
using Verbose.Data;
using Verbose.fragments;
using Verbose.src.adapters;

namespace Verbose
{
    public class SearchResultFragment : AndroidX.Fragment.App.Fragment
    {
        VerboseAPIService _api;

        RecyclerView.LayoutManager mLayoutManager;

        private bool searchActive = false;

        View view;

        SearchView searchBar;
        RecyclerView searchResultRecycler;
        PodcastCardAdapter searchResultAdapter;
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
            view = inflater.Inflate(Resource.Layout.search_results_page, container, false);

            base.OnCreate(savedInstanceState);

            progressSpinner = view.FindViewById<ProgressBar>(Resource.Id.feed_progress_bar);
            progressSpinner.Visibility = ViewStates.Gone;

            savedInstanceState = this.Arguments;
            

            // Get the api
            _api = VerboseAPIService.Instance;

            searchBar = view.FindViewById<SearchView>(Resource.Id.search_bar);
            searchBar.QueryTextSubmit += SearchBarSubmit;
            string temp = savedInstanceState.GetString("search");
            searchBar.SetQuery(temp, false);

            progressSpinner = view.FindViewById<ProgressBar>(Resource.Id.feed_progress_bar);

            searchBar = view.FindViewById<SearchView>(Resource.Id.search_bar);
            searchBar.QueryTextSubmit += SearchBarSubmit;

            searchResultRecycler = view.FindViewById<RecyclerView>(Resource.Id.search_results_recycler);
            mLayoutManager = new LinearLayoutManager(Context);
            searchResultRecycler.SetLayoutManager(mLayoutManager);

            searchResultAdapter = new PodcastCardAdapter();
            searchResultAdapter.ItemClick += OpenPodcastPage;
            searchResultAdapter.podcastList = _api.searchResults;

            searchResultRecycler.SetAdapter(searchResultAdapter);

            return view;
        }

        private async void SearchBarSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            if (!searchActive)
            {
                searchActive = true;
                progressSpinner.Visibility = ViewStates.Visible;

                SearchView sv = sender as SearchView;
                string text = sv.Query;

                if (await _api.SearchPodcasts(text))
                {
                    searchResultAdapter.podcastList = _api.searchResults;
                    searchResultAdapter.NotifyDataSetChanged();
                }
                else
                {
                    Toast.MakeText(Context, "Search Failed on the Verbose Server", ToastLength.Short).Show();
                }

                progressSpinner.Visibility = ViewStates.Gone;
                searchActive = false;
            }
        }

        public void OpenPodcastPage(object sender, int position)
        {
            progressSpinner.Visibility = ViewStates.Visible;
            PodcastCardAdapter adapter = (PodcastCardAdapter)sender;
            Podcast podcast = adapter.podcastList[position];
            PodcastParcelable parcelable = new PodcastParcelable();
            parcelable.podcast = podcast;

            PodcastPageFragment podcastFragment = new PodcastPageFragment();
            Bundle bundle = new Bundle();
            bundle.PutParcelable("Podcast", parcelable);

            podcastFragment.Arguments = bundle;

            // Send the url of the podcast
            ((MainPageActivity)Activity).ChangeFragment(podcastFragment, Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_slide_out_bottom);
        }
    }
}
