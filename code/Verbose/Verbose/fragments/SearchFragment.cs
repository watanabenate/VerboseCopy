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
//using Verbose.src.adapters;

namespace Verbose
{
    public class SearchFragment : AndroidX.Fragment.App.Fragment
    {
        VerboseAPIService _api;

        RecyclerView.LayoutManager mLayoutManager;

        private bool searchActive = false;

        View view;

        SearchView searchBar;
        ProgressBar progressSpinner;

        private readonly List<string> categories = new List<string>()
        {
            "Comedy",
            "TrueCrime",
            "News",
            "Sports",
            "TV"
        };

        List<RecommendationCardAdapter> adapters;

        int[] recycler_ids =
                {Resource.Id.comedy_recycler,
                Resource.Id.true_crime_recycler,
                Resource.Id.news_recycler,
                Resource.Id.sports_recycler,
                Resource.Id.tv_movie_recycler};

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
            view = inflater.Inflate(Resource.Layout.search_page, container, false);

            base.OnCreate(savedInstanceState);

            progressSpinner = view.FindViewById<ProgressBar>(Resource.Id.feed_progress_bar);
            progressSpinner.Visibility = ViewStates.Visible;

            // Get the api
            _api = VerboseAPIService.Instance;

            adapters = new List<RecommendationCardAdapter>();

            // Set up adapters for search recommendations
            for (int i = 0; i < recycler_ids.Length; i++)
            {
                int category_id = recycler_ids[i];
                adapters.Add(new RecommendationCardAdapter());

                RecyclerView currRecycler = view.FindViewById<RecyclerView>(category_id);
                mLayoutManager = new LinearLayoutManager(Context, LinearLayoutManager.Horizontal, false);
                currRecycler.SetLayoutManager(mLayoutManager);

                currRecycler.SetAdapter(adapters[i]);
            }

            // Set up search functionality
            searchBar = view.FindViewById<SearchView>(Resource.Id.search_bar);
            searchBar.QueryTextSubmit += SearchBarSubmit;

            GetSearchRecommendations();

            return view;
        }

        private async void GetSearchRecommendations()
        {
            if(_api.SearchPageRecommendations.Count == 0)
            {
                await Task.Run(_api.GetSearchPageRecommendations); // Run this on a separate thread so we can still search
            }

            for (int i = 0; i < categories.Count; i++)
            {
                string category = categories[i];
                RecommendationCardAdapter adapter = adapters[i];

                adapter.podcastList = _api.SearchPageRecommendations[category];
                // Tell the adapter to update
                adapter.ItemClick += OpenPodcastPage;
                adapter.NotifyDataSetChanged();
            }

            progressSpinner.Visibility = ViewStates.Gone;
        }

        private async void SearchBarSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            if (!searchActive)
            {
                searchActive = true;
                progressSpinner.Visibility = ViewStates.Visible;

                SearchView sv = sender as SearchView;
                string text = sv.Query;

                if (await Task.Run(() => { return _api.SearchPodcasts(text); })) // Run this on a separate thread so we can still interact with the app
                {
                    SetSearchResultsPage();
                }
                else
                {
                    Toast.MakeText(Context, "Search Failed on the Verbose Server", ToastLength.Short).Show();
                    progressSpinner.Visibility = ViewStates.Gone;
                }

                searchActive = false;
            }
        }

        private void SetSearchResultsPage()
        {
            progressSpinner.Visibility = ViewStates.Visible;

            SearchResultFragment resultFragment = new SearchResultFragment();

            Bundle bundle = new Bundle();
            bundle.PutString("search", searchBar.Query);
            resultFragment.Arguments = bundle;

            // Send the url of the podcast
            ((MainPageActivity)Activity).ChangeFragment(resultFragment);
        }

        public void OpenPodcastPage(object sender, int position)
        {
            progressSpinner.Visibility = ViewStates.Visible;
            RecommendationCardAdapter adapter = (RecommendationCardAdapter)sender;
            Podcast podcast = adapter.podcastList[position];
            PodcastParcelable parcelable = new PodcastParcelable();
            parcelable.podcast = podcast;

            PodcastPageFragment podcastFragment = new PodcastPageFragment();
            Bundle bundle = new Bundle();
            bundle.PutParcelable("Podcast", parcelable);

            podcastFragment.Arguments = bundle;

            // Send the url of the podcast
            ((MainPageActivity)Activity).ChangeFragment(podcastFragment);
        }
    }
}
