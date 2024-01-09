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
using Android.Graphics.Drawables;

namespace Verbose
{
    public class SttPlayerFragment : AndroidX.Fragment.App.Fragment, IAudioPlayer, MediaPlayer.IOnPreparedListener
    {
        const long SKIP_TIME = 10000;

        public event EventHandler audioPlaybackComplete;

        // The Player for our audio
        // see: https://developer.android.com/reference/android/media/MediaPlayer
        private MediaPlayer _mediaPlayer;
        // The audio url for our audio
        private string _mediaURL;

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

        // Closed captions
        TextView captionText;
        Dictionary<int, string> captionDict;

        VerboseAPIService _api;
        View view;

        ProgressBar progressSpinner;

        private bool canUpdateSeekbar;
        private bool captionsVisible;

        /// <summary>
        /// This method is always called when the activity (page) is first created.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            container.RemoveAllViews();
            // Set our view from the "main" layout resource
            view = inflater.Inflate(Resource.Layout.stt_player_page, container, false);

            // Get the api
            _api = VerboseAPIService.Instance;

            canUpdateSeekbar = true;

            // Set audio link to transcription clip
            MediaURL = "https://homepage.ntu.edu.tw/~karchung/miniconversations/mc27.mp3";

            progressSpinner = view.FindViewById<ProgressBar>(Resource.Id.player_progress_bar);
            progressSpinner.Visibility = ViewStates.Visible;

            // Set up captions
            captionText = view.FindViewById<TextView>(Resource.Id.stt_closed_captions);
            captionDict = _api.captionDict;
            captionsVisible = false;

            // Setting up the seekbar
            mediaPlayerSeekBar = view.FindViewById<SeekBar>(Resource.Id.stt_mediaPlayerSeekbar);
            mediaPlayerSeekBar.StopTrackingTouch += SeekBarStopTrackingTouch;
            mediaPlayerSeekBar.StartTrackingTouch += SeekBarStartTrackingTouch;
            seekbarHandler = new Handler();

            // Setup the player functionality
            playPauseButton = view.FindViewById<ImageButton>(Resource.Id.stt_play_pause_button);
            playPauseButton.Click += async delegate
            {
                if (IsPlaying) { Pause(); }
                else { Play(); }
            };

            forwardButton = view.FindViewById<ImageButton>(Resource.Id.stt_forward_button);
            forwardButton.Click += delegate
            {
                Forward();
            };

            backwardButton = view.FindViewById<ImageButton>(Resource.Id.stt_back_button);
            backwardButton.Click += delegate
            {
                Rewind();
            };

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

            EpisodeImage = view.FindViewById<ImageView>(Resource.Id.stt_episode_image);
            // Add image to the player
            EpisodeImage.SetImageDrawable(Context.GetDrawable(Resource.Drawable.mic_coverart));

            // Add listener to image to display captions
            EpisodeImage.Click += DisplayCaptions;

            // Add episode and creator name to the player
            EpisodeTitle = view.FindViewById<TextView>(Resource.Id.stt_scrolling_episode_title);
            CreatorName = view.FindViewById<TextView>(Resource.Id.stt_creator_name);
            EpisodeTitle.Text = "Speech-To-Text Demo";
            EpisodeTitle.Selected = true;
            CreatorName.Text = "Tap the coverart to display captions";

            _currentTime = view.FindViewById<TextView>(Resource.Id.stt_currentTimePlayer);
            _endTime = view.FindViewById<TextView>(Resource.Id.stt_endTimePlayer);

            // Set the current time and progress of the podcast
            _currentTime.Text = _api.getTimeString(_mediaPlayer.CurrentPosition);
            _endTime.Text = _api.getTimeString(_mediaPlayer.Duration);

            updateCaptions();

            ((MainPageActivity)Activity).ResetPlayBar();

            // Add back button
            view.FindViewById<ImageButton>(Resource.Id.stt_min_player).Click += (o, s) => ((MainPageActivity)Activity).OnBackPressed();

            return view;
        }

        /// <summary>
        /// This method is responsible for setting up the player by loading the url, 
        /// or if something is already playing it will setup the seekbar and play/pause button
        /// </summary>
        public bool SetupPlayer()
        {
            _mediaPlayer = _api.Player;

            // If we have a player, make sure to stop what is
            // currently playing
            if (_mediaPlayer.IsPlaying)
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

        public void OnPrepared(MediaPlayer mp)
        {
            _mediaPlayer.SeekTo(0, MediaPlayerSeekMode.PreviousSync);

            // Set the _api player to show the correct one
            _api.Player = _mediaPlayer;
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

        public void Forward()
        {
            long currPos = _mediaPlayer.CurrentPosition;
            currPos += SKIP_TIME;
            _mediaPlayer.SeekTo(currPos, MediaPlayerSeekMode.NextSync);
            _currentTime.Text = _api.getTimeString(_mediaPlayer.CurrentPosition);
            mediaPlayerSeekBar.SetProgress(_mediaPlayer.CurrentPosition, true);

            // Reset live comments (easiest way, slow, change in future)
            updateCaptions();
        }

        public void Rewind()
        {
            long currPos = _mediaPlayer.CurrentPosition;
            currPos -= SKIP_TIME;
            _mediaPlayer.SeekTo(currPos, MediaPlayerSeekMode.PreviousSync);
            _currentTime.Text = _api.getTimeString(_mediaPlayer.CurrentPosition);
            mediaPlayerSeekBar.SetProgress(_mediaPlayer.CurrentPosition, true);

            // Reset live comments (easiest way, slow, change in future)
            updateCaptions();
        }

        private void PlaybackComplete()
        {
            seekbarHandler.RemoveCallbacksAndMessages(null);
            IsPlaying = false;
            // Change the play/pause icon
            playPauseButton.SetImageResource(Resource.Drawable.play_button);

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
            updateCaptions();
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
            updateCaptions();
        }

        void DisplayCaptions(object sender, EventArgs e)
        {
            if (captionsVisible)
            {
                captionText.Visibility = ViewStates.Gone;
            }
            else
            {
                captionText.Visibility = ViewStates.Visible;
            }
        }

        /// <summary>
        /// This method will check to see if we need to change the current displayed captions
        /// </summary>
        void updateCaptions()
        {
            int currentPlayerPosition = _mediaPlayer.CurrentPosition;
            string text = "";

            // Assume sorted list of captions and find the earliest one that works
            foreach (int captPosition in _api.captionDict.Keys)
            {
                if (currentPlayerPosition < captPosition)
                {
                    text = _api.captionDict[captPosition];
                    break;
                }
            }
            captionText.Text = text;
        }

        /// <summary>
        /// This method is called when the like button is clicked. It will changed the button of the like
        /// </summary>
        #endregion Activity Methods

        private class CommentTimestampComparer : Comparer<Comment>
        {
            public override int Compare(Comment x, Comment y)
            {
                return x.Timestamp.CompareTo(y.Timestamp);
            }
        }

        public override void OnPause()
        {
            canUpdateSeekbar = false;
            seekbarHandler.RemoveCallbacksAndMessages(null);
            base.OnPause();
            IsPlaying = false;
            _mediaPlayer.Stop();
            _mediaPlayer.Reset();
            _api.CurrentPlayingEpisode = null;
            _api._currentPlayingEpisode = null;
        }
    }
}