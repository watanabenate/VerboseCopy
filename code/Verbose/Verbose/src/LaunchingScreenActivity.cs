using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Verbose.API;
using Verbose.Data;

namespace Verbose.src
{
    [Activity(Label = "@string/app_name", Theme = "@style/VerboseLoginTheme", MainLauncher = true, NoHistory = true)]
    public class LaunchingScreenActivity : Activity
    {
        private VerboseAPIService _api;

        FirebaseAuth fireAuth;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.launching_page);

            // Get the api
            _api = VerboseAPIService.Instance;

            if(!_api.IsConnected)
            {
                Toast.MakeText(this, "You are not connected to the internet.", ToastLength.Long).Show();
                var intent = new Intent(this, typeof(LoginScreenActivity));
                StartActivity(intent);
                Finish();
                return;
            }

            // Set up firebase auth
            fireAuth = GetFirebaseAuth();

            // check if use is already signed in
            CheckSignedIn();
        }

        private FirebaseAuth GetFirebaseAuth()
        {
            var app = FirebaseApp.InitializeApp(this);
            FirebaseAuth mAuth;

            // Make a new FirebaseApp if it has not been started up already
            // It can be not null as it is stored in the cache
            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetProjectId("verbose-capstone")
                    .SetApplicationId("verbose-capstone")
                    .SetApiKey("AIzaSyBzOXRV83tTrFYRzTIgo2A3F31LbPZ6-Ko")
                    .SetStorageBucket("verbose-capstone.appspot.com")
                    .Build();

                FirebaseApp.InitializeApp(this, options);
            }
            mAuth = FirebaseAuth.Instance;
            return mAuth;
        }

        /// <summary>
        /// This method will check if the user is already signed in
        /// </summary>
        private async void CheckSignedIn()
        {
            Intent intent;
            if (fireAuth.CurrentUser != null)
            {
                // Set the API's fireAuth and get our profile
                if (await _api.CheckMadeAccountAsync(fireAuth))
                {
                    // User signed in
                    if(await _api.GetRecentlyListenedToAsync(_api.UserProfile.PublicProfileInfo.UserName) &&
                    await _api.GetMainFeedAsync(_api.UserProfile.PublicProfileInfo.UserName))
                    {
                        // So we don't have to wait to get all of these (stores it in the episode)
                        foreach (ListenedTo listenedTo in _api.MainFeedRecentlyListenedTo)
                        {
                            _api.GetImageBitmap(listenedTo.Episode);
                        }
                    }
                    
                    

                    intent = new Intent(this, typeof(MainPageActivity));
                    StartActivity(intent);
                    Finish();
                    return;
                }
                
                if (!_api.IsConnectedToServer)
                {
                    Toast.MakeText(this, "Could not connect to the Verbose server.", ToastLength.Long).Show();
                }
                else
                {
                    // They did not have an account but FireBase thought they were logged in 
                    // for some reason
                    fireAuth.SignOut();
                }
            }
            // Send them to the login screen
            intent = new Intent(this, typeof(LoginScreenActivity));
            StartActivity(intent);
            Finish();
            return;
        }
    }
}