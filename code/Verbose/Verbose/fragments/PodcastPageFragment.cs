using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using Verbose.API;
using Verbose.Data;
using Verbose.src.adapters;

namespace Verbose.fragments
{
    public class PodcastPageFragment : AndroidX.Fragment.App.Fragment
    {
        View view;
        VerboseAPIService _api;

        ImageView podcastImage;
        TextView podcastName;
        TextView podcastDescription;
        TextView podcastCreator;

        RecyclerView episodeListRecycler;
        LinearLayoutManager mEpisodeLayoutManager;
        EpisodeInfoCardAdapter mEpisodeAdapter;

        Podcast podcast;

        ProgressBar progressBar;

        Button subOrUnsubButton;

        private bool subOrUnsubButtonActive = false;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Get the api
            _api = VerboseAPIService.Instance;

            container.RemoveAllViews();
            view = inflater.Inflate(Resource.Layout.podcast_page, container, false);

            progressBar = view.FindViewById<ProgressBar>(Resource.Id.feed_progress_bar);
            progressBar.Visibility = ViewStates.Gone;

            // Add back button
            view.FindViewById<ImageButton>(Resource.Id.min_podcast_page).Click += (o, s) => ((MainPageActivity)Activity).OnBackPressed();

            // Get the podcast we are displaying
            savedInstanceState = this.Arguments;
            PodcastParcelable temp = (PodcastParcelable) savedInstanceState.GetParcelable("Podcast");
            podcast = ((PodcastParcelable)savedInstanceState.GetParcelable("Podcast")).podcast;

            podcastImage = view.FindViewById<ImageView>(Resource.Id.podcast_image);
            podcastImage.SetImageBitmap(_api.GetImageBitmap(podcast));

            podcastName = view.FindViewById<TextView>(Resource.Id.podcast_name);
            if (podcast.Title != null)
            {
                podcastName.Text = podcast.Title;
            }
                
            podcastDescription = view.FindViewById<TextView>(Resource.Id.podcast_description);
            if(podcast.Description != null)
            {
                podcastDescription.Text = podcast.Description;
            }

            podcastCreator = view.FindViewById<TextView>(Resource.Id.podcast_creator);
            podcastCreator.Text = podcast.Creator;

            // Setup our episodes
            episodeListRecycler = view.FindViewById<RecyclerView>(Resource.Id.episode_recycler);
            mEpisodeLayoutManager = new LinearLayoutManager(Context);
            episodeListRecycler.SetLayoutManager(mEpisodeLayoutManager);
            mEpisodeAdapter = new EpisodeInfoCardAdapter(podcast.Episodes);
            mEpisodeAdapter.ItemClick += PlayPodcast;
            episodeListRecycler.SetAdapter(mEpisodeAdapter);

            subOrUnsubButton = view.FindViewById<Button>(Resource.Id.sub_or_unsub_btn);
            if(_api.UserProfile.PublicProfileInfo.Subscribed.Contains(podcast)) // Might need to do a HashTable here
            {
                subOrUnsubButton.Text = "UNSUBSCRIBE";
            }
            subOrUnsubButton.Click += SubOrUnsubPodcast;

            return view;
        }

        private async void SubOrUnsubPodcast(object sender, EventArgs e)
        {
            if (subOrUnsubButtonActive) return;

            progressBar.Visibility = ViewStates.Visible;

            subOrUnsubButtonActive = true;

            bool subOrUnsub = subOrUnsubButton.Text == "SUBSCRIBE" ? true : false;

            if(await _api.SubOrUnsubPodcast(subOrUnsub, podcast))
            {
                if(subOrUnsub)
                {
                    subOrUnsubButton.Text = "UNSUBSCRIBE";
                }
                else
                {
                    subOrUnsubButton.Text = "SUBSCRIBE";
                }
            }
            else
            {
                Toast.MakeText(Context, "Could not subscribe/unsubscribe from podcast.", ToastLength.Short).Show();
            }

            progressBar.Visibility = ViewStates.Gone;

            subOrUnsubButtonActive = false;
        }

        private async void PlayPodcast(object sender, int position)
        {
            progressBar.Visibility = ViewStates.Visible;

            EpisodeInfoCardAdapter adapter = (EpisodeInfoCardAdapter)sender;
            PodcastEpisode episode = adapter.mEpisodeList[position];

            // See if the user has listened to this podcast before
            if(await _api.GetUserListenedTo(episode))
            {
                ListenedToParcelable parcelable = new ListenedToParcelable();
                parcelable.listenedTo = _api.userListenedTo;

                AudioPlayerFragment audioFragment = new AudioPlayerFragment();
                Bundle bundle = new Bundle();
                bundle.PutParcelable("ListenedTo", parcelable);

                audioFragment.Arguments = bundle;

                // Send the url of the podcast
                ((MainPageActivity)Activity).ChangeFragment(audioFragment, "audio");
            }
            else
            {
                Toast.MakeText(Context, "Could not load if you listened to this podcast.", ToastLength.Short).Show();
                progressBar.Visibility = ViewStates.Gone;
            }
            
        }
    }
}