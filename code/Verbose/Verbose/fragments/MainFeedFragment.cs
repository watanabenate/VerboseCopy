using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using Verbose.API;
using Verbose.Data;
using Verbose.fragments;
using Verbose.src.adapters;
using static Android.Views.ViewGroup;

namespace Verbose
{
    public class MainFeedFragment : AndroidX.Fragment.App.Fragment
    {
        VerboseAPIService _api;

        View view;

        RecyclerView postListRecycler;
        LinearLayoutManager mPostLayoutManager;
        PostCardAdapter mPostAdapter;

        RecyclerView episodeListRecycler;
        LinearLayoutManager mEpisodeLayoutManager;
        EpisodeCardAdapter mEpisodeAdapter;

        ImageButton newPostButton, friendSearchButton;

        ProgressBar progressSpinner;

        TextView emptyRecents, emptyFeed;

        private bool gettingMainFeed = false;

        /// <summary>
        /// This is called whenever the fragment is made.
        /// </summary>
        /// <param name="inflater"></param>
        /// <param name="container"></param>
        /// <param name="savedInstanceState"></param>
        /// <returns></returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Get the api
            _api = VerboseAPIService.Instance;

            container.RemoveAllViews();
            view = inflater.Inflate(Resource.Layout.main_feed_page, container, false);

            progressSpinner = view.FindViewById<ProgressBar>(Resource.Id.feed_progress_bar);
            progressSpinner.Visibility = ViewStates.Visible;

            // - - - Empty RecyclerView warnings - - - //
            emptyRecents = view.FindViewById<TextView>(Resource.Id.empty_recents_warning);
            emptyFeed = view.FindViewById<TextView>(Resource.Id.empty_feed_warning);

            // - - - Friend Search - - - //
            friendSearchButton = view.FindViewById<ImageButton>(Resource.Id.add_friend_icon);
            friendSearchButton.Click += LaunchFriendSearch;

            // - - - Recently Listened Recycler View - - - //
            episodeListRecycler = view.FindViewById<RecyclerView>(Resource.Id.recents_recycler);

            mEpisodeLayoutManager = new LinearLayoutManager(Context, LinearLayoutManager.Horizontal, false);
            episodeListRecycler.SetLayoutManager(mEpisodeLayoutManager);


            // - - - Main Posts Recycler View - - - //
            postListRecycler = view.FindViewById<RecyclerView>(Resource.Id.feed_post_recycler);

            mPostLayoutManager = new LinearLayoutManager(Context);

            mPostAdapter = new PostCardAdapter();
            mPostAdapter.PostClick += GoToPost;
            mPostAdapter.ProfileClick += GoToProfile;
            mPostAdapter.EpisodeClick += GoToPostPodcast;

            var onScrollListener = new XamarinRecyclerViewOnScrollListener(mPostLayoutManager);
            onScrollListener.LoadMoreEvent += (object sender, EventArgs e) =>
            {
                //Load more stuff here
                GetMainFeed();
            };
            postListRecycler.AddOnScrollListener(onScrollListener);

            postListRecycler.SetLayoutManager(mPostLayoutManager);
            postListRecycler.SetAdapter(mPostAdapter);

            newPostButton = view.FindViewById<ImageButton>(Resource.Id.new_post_button);
            newPostButton.Click += NewPost;

            GetRecentlyListenedTo();

            RefreshMainFeed();

            if(((MainPageActivity)Activity).playBarCanBeVisible)
            {
                ((MarginLayoutParams)newPostButton.LayoutParameters).BottomMargin = convertToDp(72);
            }
            

            return view;
        }

        private int convertToDp(int pixels)
        {
            Resources r = Context.Resources;
            int dp = (int)TypedValue.ApplyDimension(
                    ComplexUnitType.Dip,
                    pixels,
                    r.DisplayMetrics);
            return dp;
        }

        private void NewPost(object sender, EventArgs e)
        {
            NewPostFragment newPostFragment = new NewPostFragment();

            ((MainPageActivity)Activity).ChangeFragment(newPostFragment, Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_slide_out_bottom);
        }

        private void LaunchFriendSearch(object sender, EventArgs e)
        {
            FriendSearchFragment friendSearchFragment = new FriendSearchFragment();
            ((MainPageActivity)Activity).ChangeFragment(friendSearchFragment, Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_slide_out_bottom);
        }

        private void RefreshMainFeed()
        {
            _api.ReachedEndOfFeed = false;
            _api.MainFeedIndex = 0;
            _api.MainFeedPosts = new List<Post>();
            GetMainFeed();
        }

        private async void GetMainFeed()
        {
            if (_api.ReachedEndOfFeed)
                return;

            if (gettingMainFeed)
            {
                return;
            }
            gettingMainFeed = true;

            // Getting our main feed
            if (await _api.GetMainFeedAsync(_api.UserProfile.PublicProfileInfo.UserName))
            {
                progressSpinner.Visibility = ViewStates.Gone;
                int size = _api.MainFeedPosts.Count;

                // Display empty feed warning
                if (size == 0)
                    emptyFeed.Visibility = ViewStates.Visible;
                else
                {
                    emptyFeed.Visibility = ViewStates.Gone;

                    for (int i = size - _api.LoadMoreCount; i < size; i++)
                    {
                        mPostAdapter.postList.Add(_api.MainFeedPosts[i]);
                        mPostAdapter.NotifyItemInserted(mPostAdapter.postList.Count - 1);
                    }
                }
            }
            else
            {
                Toast.MakeText(Context, "Could not get main feed.", ToastLength.Short).Show();
                progressSpinner.Visibility = ViewStates.Gone;
            }

            gettingMainFeed = false;
        }

        private async void GetRecentlyListenedTo()
        {
            // Refreshing our recently listened to
            if (await _api.GetRecentlyListenedToAsync(_api.UserProfile.PublicProfileInfo.UserName))
            {
                mEpisodeAdapter = new EpisodeCardAdapter(_api.MainFeedRecentlyListenedTo);
                episodeListRecycler.SetAdapter(mEpisodeAdapter);
                mEpisodeAdapter.NotifyDataSetChanged();
                mEpisodeAdapter.ItemClick += PlayPodcast;

                // Display empty recents warning
                if (mEpisodeAdapter.ItemCount == 0)
                    emptyRecents.Visibility = ViewStates.Visible;
                else
                    emptyRecents.Visibility = ViewStates.Gone;
            }
        }

        public async void PlayPodcast(object sender, int position)
        {
            progressSpinner.Visibility = ViewStates.Visible;

            EpisodeCardAdapter adapter = (EpisodeCardAdapter)sender;
            ListenedTo listenedTo = adapter.episodeList[position];
            // See if the user has listened to this podcast before
            if (await _api.GetUserListenedTo(listenedTo.Episode))
            {
                ListenedToParcelable parcelable = new ListenedToParcelable();
                parcelable.listenedTo = _api.userListenedTo;

                AudioPlayerFragment audioFragment = new AudioPlayerFragment();
                Bundle bundle = new Bundle();
                bundle.PutParcelable("ListenedTo", parcelable);

                audioFragment.Arguments = bundle;

                // Send the url of the podcast
                ((MainPageActivity)Activity).ChangeFragment(audioFragment, Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_slide_out_bottom, "audio");
            }
            else
            {
                Toast.MakeText(Context, "Could not load if you listened to this podcast.", ToastLength.Short).Show();
                progressSpinner.Visibility = ViewStates.Gone;
            }
        }

        public void GoToPost(object sender, int position)
        {
            progressSpinner.Visibility = ViewStates.Visible;

            PostCardAdapter adapter = (PostCardAdapter)sender;
            Post post = adapter.postList[position];
            PostParcelable parcelable = new PostParcelable();
            parcelable.p = post;

            PostPageFragment postFragment = new PostPageFragment();
            Bundle bundle = new Bundle();
            bundle.PutParcelable("post", parcelable);

            postFragment.Arguments = bundle;

            ((MainPageActivity)Activity).ChangeFragment(postFragment, Resource.Animation.slide_in_right, Resource.Animation.slide_out_right);
        }

        private async void GoToPostPodcast(object sender, int position)
        {
            progressSpinner.Visibility = ViewStates.Visible;

            PostCardAdapter adapter = (PostCardAdapter)sender;
            Post post = adapter.postList[position];

            if(await _api.GetPodcastFromId(post.Episode.PodchaserPodcastID))
            {
                Podcast p = _api.PodcastFromId;

                Bundle b = new Bundle();
                b.PutParcelable("Podcast", new PodcastParcelable(p));

                // Get the other user first
                PodcastPageFragment podcastFragment = new PodcastPageFragment();
                podcastFragment.Arguments = b;
                ((MainPageActivity)Activity).ChangeFragment(podcastFragment, Resource.Animation.slide_in_right, Resource.Animation.slide_out_right);
            }
            else
            {
                Toast.MakeText(Context, "Could not load podcast", ToastLength.Short).Show();
                progressSpinner.Visibility = ViewStates.Gone;
            }
        }

        private async void GoToProfile(object sender, int position)
        {
            progressSpinner.Visibility = ViewStates.Visible;

            PostCardAdapter adapter = (PostCardAdapter)sender;
            Post post = adapter.postList[position];

            // Make sure to have a check if it's the same user or a different user
            // Don't want to navigate to other profile page if it's the current user
            if (post.Username == _api.UserProfile.PublicProfileInfo.UserName)
            {
                ProfilePageFragment profileFragment = new ProfilePageFragment();
                ((MainPageActivity)Activity).ChangeFragment(profileFragment);
            }
            else
            {
                // Get the other user first
                if (await _api.GetOtherUserPublicProfile(post.Username))
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
        public override void OnPause()
        {
            progressSpinner.Visibility = ViewStates.Gone;
            base.OnPause();
        }
    }
}