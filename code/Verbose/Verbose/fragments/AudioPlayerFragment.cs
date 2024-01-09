using Android.Media;
using Android.OS;
using Android.Widget;
using Verbose.API;
using static Android.Widget.SeekBar;
using System;
using Android.Views;
using Verbose.Data;
using AndroidX.RecyclerView.Widget;
using Verbose.src.adapters;
using Verbose.fragments;
using System.Collections.Generic;

namespace Verbose
{
    public class AudioPlayerFragment : AndroidX.Fragment.App.Fragment, IAudioPlayer, MediaPlayer.IOnPreparedListener
    {
        const long SKIP_TIME = 10000;

        public event EventHandler audioPlaybackComplete;

        // The Player for our audio
        // see: https://developer.android.com/reference/android/media/MediaPlayer
        private MediaPlayer _mediaPlayer;
        // The audio url for our audio
        private string _mediaURL;
        private PodcastEpisode episode;
        private ListenedTo listenedTo;
        private List<Comment> undisplayedComments;
        private List<Comment> displayedComments;

        TextView _currentTime;
        TextView _endTime;

        /// <summary>
        /// Our variables for the player
        /// </summary>
        ImageButton playPauseButton;
        ImageButton forwardButton;
        ImageButton backwardButton;

        // Seekbar
        SeekBar mediaPlayerSeekBar;
        Handler seekbarHandler;

        ImageView EpisodeImage;
        TextView EpisodeTitle;
        TextView CreatorName;

        // Variables for liking the podcast
        ImageButton LikeButton;
        private bool isLiked;
        TextView LikeCount;
        private bool likeActive = false;

        // Comment count display
        TextView CommentCount;

        ImageButton ShareButton;

        // Comment submission
        EditText commentText;
        ImageButton commentSubmissionButton;
        private bool commentButtonActive = false;

        // Comment recycler view
        RecyclerView commentListRecycler;
        LinearLayoutManager mCommentLayoutManager;
        CommentCardAdapter mCommentAdapter;

        ProgressBar progressSpinner;

        VerboseAPIService _api;
        View view;

        private bool canUpdateSeekbar;

        /// <summary>
        /// This method is always called when the activity (page) is first created.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            container.RemoveAllViews();
            // Set our view from the "main" layout resource
            view = inflater.Inflate(Resource.Layout.audio_player_page, container, false);

            // Get the api
            _api = VerboseAPIService.Instance;

            canUpdateSeekbar = true;

            progressSpinner = view.FindViewById<ProgressBar>(Resource.Id.player_progress_bar);
            progressSpinner.Visibility = ViewStates.Visible;

            // Get our URL for the audio
            savedInstanceState = this.Arguments;
            listenedTo = ((ListenedToParcelable)savedInstanceState.GetParcelable("ListenedTo")).listenedTo;
            if(listenedTo == null)
            {
                // Panic
                Toast.MakeText(Context, "Listened To for episode is NULL", ToastLength.Short).Show();
                ((MainPageActivity)Activity).OnBackPressedWait();
                return view;
            }
            episode = listenedTo.Episode;
            episode.Comments.Sort(new CommentTimestampComparer());

            

            MediaURL = episode.PlayLink;

            // Setting up the seekbar
            mediaPlayerSeekBar = view.FindViewById<SeekBar>(Resource.Id.mediaPlayerSeekbar);
            mediaPlayerSeekBar.StopTrackingTouch += SeekBarStopTrackingTouch;
            mediaPlayerSeekBar.StartTrackingTouch += SeekBarStartTrackingTouch;
            seekbarHandler = new Handler();

            // Setup the player functionality
            playPauseButton = view.FindViewById<ImageButton>(Resource.Id.play_pause_button);
            playPauseButton.Click += async delegate
            {
                if (IsPlaying) { Pause(); }
                else { Play(); }

                // Send API the updated timestamp
                UpdateRecentlyListenedToAsync();
            };

            // Get all of our media player controls that will need to be disabled
            // while the media player is loading
            _currentTime = view.FindViewById<TextView>(Resource.Id.currentTimePlayer);
            _endTime = view.FindViewById<TextView>(Resource.Id.endTimePlayer);
            commentText = view.FindViewById<EditText>(Resource.Id.comment_text_input);
            commentSubmissionButton = view.FindViewById<ImageButton>(Resource.Id.comment_text_submit);
            backwardButton = view.FindViewById<ImageButton>(Resource.Id.back_button);
            forwardButton = view.FindViewById<ImageButton>(Resource.Id.forward_button);
            commentText.Enabled = false;
            commentSubmissionButton.Enabled = false;
            playPauseButton.Enabled = false;
            backwardButton.Enabled = false;
            forwardButton.Enabled = false;
            mediaPlayerSeekBar.Enabled = false;

            // Setup the player to load the audio
            if (!SetupPlayer())
            {
                // If for some reason we can't setup, then just return early.
                ((MainPageActivity)Activity).OnBackPressedWait();
                return view;
            }

            forwardButton.Click += delegate
            {
                Forward();
            };

            backwardButton.Click += delegate
            {
                Rewind();
            };

            EpisodeImage = view.FindViewById<ImageView>(Resource.Id.episode_image);
            // Add image to the player
            var imageBitmap = _api.GetImageBitmap(episode);
            EpisodeImage.SetImageBitmap(imageBitmap);

            // Add episode and creator name to the player
            EpisodeTitle = view.FindViewById<TextView>(Resource.Id.scrolling_episode_title);
            CreatorName = view.FindViewById<TextView>(Resource.Id.creator_name);
            EpisodeTitle.Text = episode.Title;
            EpisodeTitle.Selected = true;
            if (episode.Creator != null)
                CreatorName.Text = episode.Creator;

            EpisodeImage.Click += GoToPodcastPage;
            EpisodeTitle.Click += GoToPodcastPage;
            CreatorName.Click += GoToPodcastPage;

            // Like button
            LikeButton = view.FindViewById<ImageButton>(Resource.Id.likeButton);
            LikeButton.Click += delegate
            {
                ClickLike();
            };
            isLiked = episode.LikedBy.Contains(_api.UserProfile.PublicProfileInfo.PublicProfileId);
            if(isLiked) { LikeButton.SetImageResource(Resource.Drawable.filled_like); } // Set it to display the full heart if we already liked it
            LikeCount = view.FindViewById<TextView>(Resource.Id.audioPlayerLikeCount);
            LikeCount.Text = episode.LikeCount.ToString();

            CommentCount = view.FindViewById<TextView>(Resource.Id.episode_comment_count);
            CommentCount.Text = episode.Comments.Count.ToString();

            ShareButton = view.FindViewById<ImageButton>(Resource.Id.audioPlayerShareButton);
            ShareButton.Click += SharePodcast;

            

            // Get our comment submission
            commentSubmissionButton.Click += submitComment;

            // Setup our comments
            undisplayedComments = new List<Comment>(episode.Comments);
            displayedComments = new List<Comment>();

            commentListRecycler = view.FindViewById<RecyclerView>(Resource.Id.audioplayer_comment_recycler);
            mCommentLayoutManager = new LinearLayoutManager(Context);
            mCommentLayoutManager.ReverseLayout = true;
            commentListRecycler.SetLayoutManager(mCommentLayoutManager);
            mCommentAdapter = new CommentCardAdapter(displayedComments, true);
            commentListRecycler.SetAdapter(mCommentAdapter);
            mCommentAdapter.ProfileClickHandler += GoToProfileComment;

            updateLiveComments();

            // Add back button
            view.FindViewById<ImageButton>(Resource.Id.min_player).Click += (o, s) => ((MainPageActivity)Activity).OnBackPressed();

            // Tell the main page we loaded this podcast and to have the play bar ready
            ((MainPageActivity)Activity).SetupPlayBar(listenedTo);

            return view;
        }

        /// <summary>
        /// Tell the api our current position in the podcast
        /// </summary>
        private async void UpdateRecentlyListenedToAsync()
        {
            if(!await _api.UpdateRecentlyListenedTo(episode, _mediaPlayer.CurrentPosition))
            {
                Toast.MakeText(Context, "Could not update your recently listened to.", ToastLength.Short).Show();
            }
        }

        /// <summary>
        /// This method is responsible for setting up the player by loading the url, 
        /// or if something is already playing it will setup the seekbar and play/pause button
        /// </summary>
        public bool SetupPlayer()
        {
            _mediaPlayer = _api.Player;


            // If the current media is this URL, don't do anything
            if(_api.CurrentPlayingMediaURL == MediaURL)
            {
                // Set up the seekbar still
                mediaPlayerSeekBar.Max = _mediaPlayer.Duration;
                mediaPlayerSeekBar.Min = 0;
                mediaPlayerSeekBar.SetProgress(_mediaPlayer.CurrentPosition, true);

                // Send API the updated timestamp
                UpdateRecentlyListenedToAsync();

                SetupPlayControls();

                // If something is playing still, we need to update the play button and seekbar
                if (IsPlaying)
                {
                    // Seekbar setup
                    StartSeekbar();
                    // Change the play/pause icon
                    playPauseButton.SetImageResource(Resource.Drawable.pause_button);
                }

                return true;
            }
            else
            {
                if(_api.CurrentPlayingEpisode != null && _mediaPlayer.IsPlaying)
                {
                    UpdateRecentlyListenedToAsync(); // Tell the API where we were at with the previous episode
                }
            }

            // If we have a player, make sure to stop what is
            // currently playing
            if(_mediaPlayer.IsPlaying)
            {
                IsPlaying = false;
                _mediaPlayer.Stop();
            }

            // Reset our player so we can start anew
            _mediaPlayer.Release();
            _mediaPlayer = new MediaPlayer();


            try
            {
                _mediaPlayer.SetDataSource(MediaURL);
            }
            catch (Exception e)
            {
                // Somehow display error to user
                Console.WriteLine(e.Message);
            }
            try
            {
                _mediaPlayer.SetOnPreparedListener(this);
                _mediaPlayer.PrepareAsync();

                // Set what happens when the playback completes
                _mediaPlayer.Completion += delegate
                {
                    PlaybackComplete();
                };

                return true;
            }
            catch (Exception e)
            {
                Toast.MakeText(Context, "Could not load podcast", ToastLength.Long).Show();
                return false;
            }
        }

        /// <summary>
        /// The callback when the mediaplayer is prepared
        /// </summary>
        /// <param name="mp"></param>
        public void OnPrepared(MediaPlayer mp)
        {
            _mediaPlayer.SeekTo(listenedTo.Timestamp, MediaPlayerSeekMode.PreviousSync);

            // Set the _api player to show the correct one
            _api.Player = _mediaPlayer;
            _api.CurrentPlayingEpisode = episode;

            updateLiveComments();

            SetupPlayControls();
        }

        /// <summary>
        /// A helper to enable all of our controls when the player is loaded
        /// </summary>
        private void SetupPlayControls()
        {
            // Seekbar
            mediaPlayerSeekBar.Max = _mediaPlayer.Duration;
            mediaPlayerSeekBar.Min = 0;
            mediaPlayerSeekBar.SetProgress(_mediaPlayer.CurrentPosition, false);

            // Set the current time and progress of the podcast
            _currentTime.Text = _api.getTimeString(_mediaPlayer.CurrentPosition);
            _endTime.Text = _api.getTimeString(_mediaPlayer.Duration);

            // Enable comments, play, back, forward, 
            commentText.Enabled = true;
            commentSubmissionButton.Enabled = true;
            playPauseButton.Enabled = true;
            backwardButton.Enabled = true;
            forwardButton.Enabled = true;
            mediaPlayerSeekBar.Enabled = true;

            progressSpinner.Visibility = ViewStates.Gone;
        }

        #region Properties
        /// <summary>
        /// This shows if the player is currently playing something
        /// </summary>
        public bool IsPlaying
        {
            get { return _api.IsPlaying; }
            set { _api.IsPlaying = value; }
        }
        /// <summary>
        /// The media that the player is using
        /// </summary>
        public string MediaURL
        {
            get { return _mediaURL; }
            set { _mediaURL = value; }
        }
        #endregion Properties

        #region Activity Methods
        /// <summary>
        /// Starts the play of the media
        /// </summary>
        /// <param name="url"></param>
        public void Play()
        {
            _mediaPlayer.Start();

            IsPlaying = true;

            // Seekbar setup
            StartSeekbar();


            // Change the play/pause icon
            playPauseButton.SetImageResource(Resource.Drawable.pause_button);
        }
        /// <summary>
        /// Pauses the media currently playing
        /// </summary>
        public void Pause()
        {
            _mediaPlayer.Pause();
            seekbarHandler.RemoveCallbacksAndMessages(null);

            IsPlaying = false;

            // Change the play/pause icon
            playPauseButton.SetImageResource(Resource.Drawable.play_button);
        }
        /// <summary>
        /// Stops the media completely
        /// </summary>
        public void Stop()
        {
            _mediaPlayer.Stop();
            seekbarHandler.RemoveCallbacksAndMessages(null);

            IsPlaying = false;
        }

        /// <summary>
        /// Go forward in the podcast
        /// </summary>
        public void Forward()
        {
            long currPos = _mediaPlayer.CurrentPosition;
            currPos += SKIP_TIME;
            _mediaPlayer.SeekTo(currPos, MediaPlayerSeekMode.NextSync);
            _currentTime.Text = _api.getTimeString(_mediaPlayer.CurrentPosition);
            mediaPlayerSeekBar.SetProgress(_mediaPlayer.CurrentPosition, true);

            // Reset live comments (easiest way, slow, change in future)
            resetComments();
            updateLiveComments();
        }
        
        /// <summary>
        /// Go backward in the podcast
        /// </summary>
        public void Rewind()
        {
            long currPos = _mediaPlayer.CurrentPosition;
            currPos -= SKIP_TIME;
            _mediaPlayer.SeekTo(currPos, MediaPlayerSeekMode.PreviousSync);
            _currentTime.Text = _api.getTimeString(_mediaPlayer.CurrentPosition);
            mediaPlayerSeekBar.SetProgress(_mediaPlayer.CurrentPosition, true);

            // Reset live comments (easiest way, slow, change in future)
            resetComments();
            updateLiveComments();
        }

        /// <summary>
        /// Called when the podcast has been listened to the entire way
        /// </summary>
        private void PlaybackComplete()
        {
            seekbarHandler.RemoveCallbacksAndMessages(null);
            IsPlaying = false;
            // Change the play/pause icon
            playPauseButton.SetImageResource(Resource.Drawable.play_button);

            // Send API the updated timestamp
            UpdateRecentlyListenedToAsync();

            audioPlaybackComplete?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// This method is responsible for updating the media player 
        /// from the seekbar when the user lifts up their finger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SeekBarStopTrackingTouch(object sender, StopTrackingTouchEventArgs e)
        {
            _mediaPlayer.SeekTo(((SeekBar)sender).Progress, MediaPlayerSeekMode.NextSync);
            // Update the time on the bar
            _currentTime.Text = _api.getTimeString(_mediaPlayer.CurrentPosition);
            if (IsPlaying)
            {
                StartSeekbar();
            }
            // Reset live comments (easiest way)
            resetComments();
            updateLiveComments();

            // Send API the updated timestamp
            UpdateRecentlyListenedToAsync();
        }

        /// <summary>
        /// This method will prevent the seekbar from moving while the user is using it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SeekBarStartTrackingTouch(object sender, StartTrackingTouchEventArgs e)
        {
            seekbarHandler.RemoveCallbacksAndMessages(null);
        }

        /// <summary>
        /// Starts the updating on seekbar (a thread will manage this)
        /// </summary>
        private void StartSeekbar()
        {
            // Seekbar setup
            mediaPlayerSeekBar.SetProgress(_mediaPlayer.CurrentPosition, true);
            seekbarHandler.PostDelayed(updateSeekbar, 150);
        }

        /// <summary>
        /// This will be used by the handler thread to update the current value of the seekbar
        /// </summary>
        void updateSeekbar()
        {
            if (!canUpdateSeekbar) return;

            _currentTime.Text = _api.getTimeString(_mediaPlayer.CurrentPosition);
            mediaPlayerSeekBar.SetProgress(_mediaPlayer.CurrentPosition, true);
            seekbarHandler.PostDelayed(updateSeekbar, 150);

            // Check for new comments here :)
            updateLiveComments();
        }

        /// <summary>
        /// This method will check to see if we need to add any new comments
        /// to the adapter
        /// </summary>
        void updateLiveComments()
        {
            int currentPlayerPosition = _mediaPlayer.CurrentPosition;

            // Treat these as sorted -- undisplayedComments[0].timestamp <= undisplayedComments[1].timestamp
            while(undisplayedComments.Count != 0)
            {
                // look at the next undisplayed comment(s)
                Comment c = undisplayedComments[0];
                
                // Always add if it's less
                if(c.Timestamp <= currentPlayerPosition)
                {
                    // The comment timestamp matched, we can go ahead and add it to be displayed
                    displayedComments.Add(c);
                    mCommentAdapter.NotifyItemInserted(mCommentAdapter.commentList.Count - 1);
                    undisplayedComments.RemoveAt(0);

                    commentListRecycler.ScrollToPosition(mCommentAdapter.ItemCount - 1);
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// This method will reset the live comments displayed, and
        /// reset the undisplayed comments back to the original comments on the episode
        /// </summary>
        void resetComments()
        {
            // This method is very slow, but gets the job done for now
            undisplayedComments = new List<Comment>(episode.Comments);
            mCommentAdapter.commentList.Clear();
            mCommentAdapter.NotifyDataSetChanged();
        }
        /// <summary>
        /// This method is called when the like button is clicked. It will changed the button of the like
        /// </summary>
        async void ClickLike()
        {
            if (!likeActive)
            {
                likeActive = true;
                // Check if this podcast is liked or not
                if (!isLiked)
                {
                    // Set it to liked and contact server. If we couldn't contact or if it failed, unlike
                    switchLikeButton();

                    if (!await _api.LikeOrUnlikePodcastEpisode(episode, true))
                    {
                        switchLikeButton();
                        Toast.MakeText(Context, "Error on the server with liking", ToastLength.Short).Show();
                    }
                }
                else
                {
                    switchLikeButton();

                    if (!await _api.LikeOrUnlikePodcastEpisode(episode, false))
                    {
                        switchLikeButton();
                        Toast.MakeText(Context, "Error on the server with un-liking", ToastLength.Short).Show();
                    }

                }
                likeActive = false;
            }
        }

        /// <summary>
        /// Makes the like button switch from filled to empty and vice-versa
        /// </summary>
        void switchLikeButton()
        {
            if(isLiked)
            {
                isLiked = false;
                LikeButton.SetImageResource(Resource.Drawable.empty_like);
                episode.LikeCount -= 1;
                LikeCount.Text = episode.LikeCount.ToString();
                episode.LikedBy.Remove(_api.UserProfile.PublicProfileInfo.PublicProfileId);
            }
            else
            {
                isLiked = true;
                LikeButton.SetImageResource(Resource.Drawable.filled_like);
                episode.LikeCount += 1;
                LikeCount.Text = episode.LikeCount.ToString();
                episode.LikedBy.Add(_api.UserProfile.PublicProfileInfo.PublicProfileId);
            }
        }

        /// <summary>
        /// Takes you to the post page to share the podcast
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SharePodcast(object sender, EventArgs e)
        {
            // Navigate to the share podcast page
            PodcastEpisodeParcelable parcelable = new PodcastEpisodeParcelable(episode);

            SharePodcastFragment sharePodcastFragment = new SharePodcastFragment();
            Bundle bundle = new Bundle();
            bundle.PutParcelable("Podcast", parcelable);

            sharePodcastFragment.Arguments = bundle;
            ((MainPageActivity)Activity).ChangeFragment(sharePodcastFragment, Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_slide_out_bottom);
        }

        /// <summary>
        /// Submits comments to our database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void submitComment(object sender, EventArgs e)
        {
            // Check if they have anything yet
            if(commentText.Text.Length <= 0)
            {
                Toast.MakeText(Context, "Please submit some text before submitting a comment.", ToastLength.Short).Show();
                return;
            }
            // Cannot be over a certain length
            else if(commentText.Text.Length > 200)
            {
                Toast.MakeText(Context, "Text is too long (needs to be under 200 characters).", ToastLength.Short).Show();
                return;
            }

            // This is so we don't keep sending to the server the same comment
            if (commentButtonActive) { return; }
            commentButtonActive = true;

            // Send to the server
            Comment c = new Comment
            {
                Username = _api.UserProfile.PublicProfileInfo.UserName,
                ProfileImageLink = _api.UserProfile.PublicProfileInfo.PictureLink,
                ProfileID = _api.UserProfile.PublicProfileInfo.PublicProfileId,
                Text = commentText.Text,
                Likes = 0,
                Timestamp = _mediaPlayer.CurrentPosition,
                Date = DateTime.Now
            };

            if(await _api.SubmitComment(c, episode))
            {
                // Add to the recylcer view
                episode.Comments.Add(c);
                episode.Comments.Sort(new CommentTimestampComparer());
                resetComments();
                updateLiveComments();

                Toast.MakeText(Context, "Comment submitted successfully.", ToastLength.Short).Show();
                commentText.Text = "";

                CommentCount.Text = episode.Comments.Count.ToString();
            }
            // Failed
            else
            {
                Toast.MakeText(Context, "Comment submission failed.", ToastLength.Short).Show();
            }

            commentButtonActive = false;
        }

        private async void GoToProfileComment(object sender, int position)
        {
            CommentCardAdapter adapter = (CommentCardAdapter)sender;
            Comment c = adapter.commentList[position];

            // Make sure to have a check if it's the same user or a different user
            // Don't want to navigate to other profile page if it's the current user
            if (c.Username == _api.UserProfile.PublicProfileInfo.UserName)
            {
                ProfilePageFragment profileFragment = new ProfilePageFragment();
                ((MainPageActivity)Activity).ChangeFragment(profileFragment);
            }
            else
            {
                // Get the other user first
                if (await _api.GetOtherUserPublicProfile(c.Username))
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

        /// <summary>
        /// Go to the podcast page for this podcast you are listening to
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GoToPodcastPage(object sender, EventArgs e)
        {
            progressSpinner.Visibility = ViewStates.Visible;

            if (await _api.GetPodcastFromId(episode.PodchaserPodcastID))
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
        #endregion Activity Methods

        private class CommentTimestampComparer : Comparer<Comment>
        {
            public override int Compare(Comment x, Comment y)
            {
                return x.Timestamp.CompareTo(y.Timestamp);
            }
        }

        public override void OnDestroyView()
        {
            canUpdateSeekbar = false;
            seekbarHandler.RemoveCallbacksAndMessages(null);
            base.OnDestroyView();
        }

        public override void OnPause()
        {
            UpdateRecentlyListenedToAsync();
            canUpdateSeekbar = false;
            if(seekbarHandler != null)
                seekbarHandler.RemoveCallbacksAndMessages(null);
            progressSpinner.Visibility = ViewStates.Gone;
            base.OnPause();
        }

        public override void OnResume()
        {
            if(IsPlaying)
            {
                canUpdateSeekbar = true;
                StartSeekbar();
            }
            base.OnResume();
        }
    }
}