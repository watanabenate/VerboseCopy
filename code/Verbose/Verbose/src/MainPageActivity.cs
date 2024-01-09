using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.CardView.Widget;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;
using System;
using System.Threading.Tasks;
using Verbose.API;
using Verbose.Data;

namespace Verbose
{
    /// <summary>
    /// This class is responsible for holding the main content of whichever fragment
    /// is currently being shown. It will also hold the menu bar.
    /// </summary>
    [Activity(Theme = "@style/VerboseTheme")]
    public class MainPageActivity : AppCompatActivity
    {
        
        public BottomNavigationView bottomNavigation;

        private VerboseAPIService _api;

        private static bool darkModeOn;
        private static bool loadSettings = false;

        private ImageView podcastImage;
        private ImageView barPlayPauseButton;
        private TextView podcastName;
        private TextView podcastCreator;
        private ListenedTo lTo;
        public CardView playBar { get; private set; }
        public bool playBarCanBeVisible { get; private set; }

        /// <summary>
        /// This will first create the bottom navigation and then set us to the
        /// home (main feed) page
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main);

            // Get the api
            _api = VerboseAPIService.Instance;

            podcastImage = FindViewById<ImageView>(Resource.Id.bar_podcast_image);
            barPlayPauseButton = FindViewById<ImageView>(Resource.Id.bar_play_pause_button);
            barPlayPauseButton.Click += PauseUnpausePlayer;
            podcastName = FindViewById<TextView>(Resource.Id.bar_podcast_title);
            podcastCreator = FindViewById<TextView>(Resource.Id.bar_podcast_creator);
            playBar = FindViewById<CardView>(Resource.Id.play_bar);
            playBar.Click += GoToAudioPlayer;

            // Set the bar to gone at first
            playBar.Visibility = Android.Views.ViewStates.Gone;
            playBarCanBeVisible = false;

            // Bottom Navigation View
            bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            bottomNavigation.ItemSelected += BottomNavigationItemSelected;

            // Dark mode
            UiModeManager modeManager = (UiModeManager)GetSystemService(UiModeService);
            if (modeManager.NightMode == UiNightMode.Yes)
                darkModeOn = true;
            else
                darkModeOn = false;

            if (loadSettings)
            {
                LoadFragment(Resource.Id.profile_nav);
                bottomNavigation.SelectedItemId = Resource.Id.profile_nav;
                loadSettings = false;
            }
            else
            {
                LoadFragment(Resource.Id.home_nav);
                bottomNavigation.SelectedItemId = Resource.Id.home_nav;
            }
        }

        

        /// <summary>
        /// This is called when an item is selected on the bottom navigation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BottomNavigationItemSelected(object sender, NavigationBarView.ItemSelectedEventArgs e)
        {
            LoadFragment(e.Item.ItemId);
        }

        /// <summary>
        /// This method is called from the navigation bar. It will load one of the three fragments
        /// </summary>
        /// <param name="id"></param>
        public void LoadFragment(int id)
        {
            switch (id)
            {
                case Resource.Id.home_nav:
                    MainFeedFragment mainFeedFragment = new MainFeedFragment();
                    ChangeFragment(mainFeedFragment);
                    break;
                case Resource.Id.search_nav:
                    SearchFragment searchFragment = new SearchFragment();
                    ChangeFragment(searchFragment);
                    break;
                case Resource.Id.profile_nav:
                    ProfilePageFragment profileFragment = new ProfilePageFragment();
                    ChangeFragment(profileFragment);
                    break;
            }
        }

        /// <summary>
        /// This method is called when we click the back button on the phone. This is necessary
        /// as if the count <= 1, we don't want to go to a blank screen.
        /// </summary>
        public override void OnBackPressed()
        {
            SupportFragmentManager.ExecutePendingTransactions();
            int count = SupportFragmentManager.BackStackEntryCount;

            if (count <= 1)
            {
                Finish();
            }
            else
            {
                SupportFragmentManager.PopBackStackImmediate();
            }

            AndroidX.Fragment.App.Fragment f = SupportFragmentManager.Fragments[SupportFragmentManager.Fragments.Count - 1];
            bottomNavigation.SelectedItemId = FindSelectedNavBar(f);

            
            // Show play bar
            TogglePlayBarVisibility(!typeof(AudioPlayerFragment).IsInstanceOfType(f));
        }

        /// <summary>
        /// This is not the greatest, but it's for if we can't load a podcast and need to go
        /// back. It's a little weird but you have to return the view in the AudioPlayer
        /// and then we can go back.
        /// </summary>
        public async void OnBackPressedWait()
        {
            Task task = Task.Delay(200);

            await task;
            SupportFragmentManager.ExecutePendingTransactions();
            SupportFragmentManager.PopBackStackImmediate();

            // Show play bar
            TogglePlayBarVisibility(true);
        }

        /// <summary>
        /// This method is called from a fragment. It is useful to navigate to a page
        /// and give it information.
        /// </summary>
        /// <param name="f"></param>
        public void ChangeFragment(AndroidX.Fragment.App.Fragment f, string tag = "")
        {
            bool isAudioPlayer = false;
            if(tag != null && tag == "audio") { isAudioPlayer = true; }

            bottomNavigation.SelectedItemId = Resource.Id.invisible;

            var frag = SupportFragmentManager.BeginTransaction();
            frag.Replace(Resource.Id.content_frame, f);
            frag.AddToBackStack(null).Commit();

            // Show play bar
            TogglePlayBarVisibility(!isAudioPlayer);
            if (isAudioPlayer)
            {
                ((AudioPlayerFragment)f).audioPlaybackComplete += FinishedPlayback;
            }
            bottomNavigation.SelectedItemId = FindSelectedNavBar(f);
        }

        /// <summary>
        /// Alternate version of ChangeFragment which adds an in/out animation to the destination fragment
        /// </summary>
        /// <param name="f"></param>
        public void ChangeFragment(AndroidX.Fragment.App.Fragment f, int transitInID, int transitOutID, string tag = "")
        {
            bool isAudioPlayer = false;
            if (tag != null && tag == "audio") { isAudioPlayer = true; }

            bottomNavigation.SelectedItemId = Resource.Id.invisible;

            var frag = SupportFragmentManager.BeginTransaction();
            frag.SetCustomAnimations(transitInID, Resource.Animation.none, Resource.Animation.none, transitOutID);
            frag.Replace(Resource.Id.content_frame, f);
            frag.AddToBackStack(null).Commit();

            // Show play bar
            TogglePlayBarVisibility(!isAudioPlayer);
            if(isAudioPlayer) { 
                ((AudioPlayerFragment)f).audioPlaybackComplete += FinishedPlayback; 
            }
            bottomNavigation.SelectedItemId = FindSelectedNavBar(f);
        }

        /// <summary>
        /// This method will be called when the user clicks the logout button. They will then be logged
        /// out and taken back to the main menu screen.
        /// </summary>
        public void Logout()
        {
            _api.logout();
            
            var intent = new Intent(this, typeof(LoginScreenActivity));
            // Make sure the user cannot click the back button
            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
            StartActivity(intent);
            Finish();
        }

        /// <summary>
        /// This method will be called when the user clicks the dark mode button. The theme of the app
        /// will be switched to either night or not night depending on their current state.
        /// </summary>
        public void ToggleDarkMode()
        {
            if (darkModeOn)
            {
                AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                darkModeOn = false;
            }
            else
            {
                AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                darkModeOn = true;
            }
            loadSettings = true;
        }

        // This means we just entered the audio player. We don't want the play bar visible, but we want it ready.
        public void SetupPlayBar(ListenedTo l)
        {
            lTo = l;

            playBar.Visibility = Android.Views.ViewStates.Gone;
            playBarCanBeVisible = true;

            podcastImage.SetImageBitmap(_api.GetImageBitmap(lTo.Episode));
            barPlayPauseButton.SetImageResource(Resource.Drawable.play_button);
            podcastName.Text = lTo.Episode.Title;
            podcastCreator.Text = lTo.Episode.Creator;
        }

        public void ResetPlayBar()
        {
            lTo = null;
            playBarCanBeVisible = false;
            playBar.Visibility = Android.Views.ViewStates.Gone;
        }

        // Toggle whether we see the play bar or not. Only shows if we have started one podcast.
        private void TogglePlayBarVisibility(bool showPlayBar)
        {
            if(playBarCanBeVisible)
            {
                playBar.Visibility = showPlayBar ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone;
                if(showPlayBar && _api.IsPlaying)
                {
                    barPlayPauseButton.SetImageResource(Resource.Drawable.pause_button);
                }
                else
                {
                    barPlayPauseButton.SetImageResource(Resource.Drawable.play_button);
                }
            }
        }

        private int FindSelectedNavBar(AndroidX.Fragment.App.Fragment f)
        {
            switch (f.Id)
            {
                case Resource.Id.home_nav:
                    return Resource.Id.home_nav;
                case Resource.Id.search_nav:
                    return Resource.Id.search_nav;
                case Resource.Id.profile_nav:
                    return Resource.Id.profile_nav;
                default: return Resource.Id.invisible;
            }
        }

        private void PauseUnpausePlayer(object sender, EventArgs e)
        {
            if(_api.IsPlaying)
            {
                _api.IsPlaying = false;
                _api.Player.Pause();
                barPlayPauseButton.SetImageResource(Resource.Drawable.play_button);
            }
            else
            {
                _api.IsPlaying = true;
                _api.Player.Start();
                barPlayPauseButton.SetImageResource(Resource.Drawable.pause_button);
            }
        }

        private void GoToAudioPlayer(object sender, EventArgs e)
        {
            ListenedToParcelable parcelable = new ListenedToParcelable();
            parcelable.listenedTo = lTo;

            AudioPlayerFragment audioFragment = new AudioPlayerFragment();
            Bundle bundle = new Bundle();
            bundle.PutParcelable("ListenedTo", parcelable);

            audioFragment.Arguments = bundle;

            // Send the url of the podcast
            ChangeFragment(audioFragment, "audio");
        }

        public void FinishedPlayback(object sender, EventArgs e)
        {
            barPlayPauseButton.SetImageResource(Resource.Drawable.play_button);
        }
    }
}