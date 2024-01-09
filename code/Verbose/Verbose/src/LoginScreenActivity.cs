using Android.App;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using Verbose.API;
using Firebase.Auth;
using Firebase;
using Android.Gms.Tasks;
using Android.Content;

namespace Verbose
{
    [Activity(Label = "@string/app_name", Theme = "@style/VerboseLoginTheme")]
    public class LoginScreenActivity : AppCompatActivity, IOnSuccessListener, IOnFailureListener
    {
        private VerboseAPIService _api;

        ImageButton login;

        GoogleSignInOptions gso;
        GoogleSignInClient gsc;
        FirebaseAuth fireAuth;

        private bool signInActive;
        

        /// <summary>
        /// This method is always called when the activity (page) is first created.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Get the api
            _api = VerboseAPIService.Instance;

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.login_page);

            login = FindViewById<ImageButton>(Resource.Id.login_button);
            login.Click += Login_Click;

            // Set up google sign in
            gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestIdToken("685158227517-4p9398sggllkj7jvuqhcd068k2f5fdvd.apps.googleusercontent.com")
                .RequestEmail()
                .Build();

            // Build a GoogleSignInClient with the options specified by gso.
            gsc = GoogleSignIn.GetClient(this, gso);

            GoogleSignInClient googleSignInClient = GoogleSignIn.GetClient(this, gso);
            googleSignInClient.RevokeAccess();

            // Set up firebase auth
            fireAuth = GetFirebaseAuth();

            signInActive = false;
        }

        /// <summary>
        /// We need this method just in case we have any permissions that need to be 
        /// approved by the user.
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="permissions"></param>
        /// <param name="grantResults"></param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private FirebaseAuth GetFirebaseAuth()
        {
            var app = FirebaseApp.InitializeApp(this);
            FirebaseAuth mAuth;

            // Make a new FirebaseApp if it has not been started up already
            // It can be not null as it is stored in the cache
            if(app == null)
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
        /// This method will be called when the sign in with google button is called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Click(object sender, EventArgs e)
        {
            if (!signInActive)
            {
                signInActive = true;
                // Start the log in process with Google
                Intent signInIntent = gsc.SignInIntent;
                StartActivityForResult(signInIntent, 1);
            }
        }

        /// <summary>
        /// This method will be called when the user signs in with their Google account
        /// in the separate window
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="resultCode"></param>
        /// <param name="data"></param>
        protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            // Google Sign In Result
            if(requestCode == 1)
            {
                Task task = GoogleSignIn.GetSignedInAccountFromIntent(data);
                GoogleSignInAccount account;
                try
                {
                    account = (GoogleSignInAccount)task.GetResult(Java.Lang.Class.FromType(typeof(ApiException)));
                }
                catch (Exception e) {
                    Toast.MakeText(this, "Google Sign In Failed", ToastLength.Short);
                    return;
                }

                LoginWithFirebase(account);
            }
            // Username created
            else if(requestCode == 2)
            {
                if(data == null)
                {
                    signInActive = false;
                    return;
                }

                if(await _api.CreateUserAsync(fireAuth, data.GetStringExtra("username")))
                {
                    Toast.MakeText(this, "Login successful", ToastLength.Short).Show();
                    var intent = new Intent(this, typeof(MainPageActivity));
                    StartActivity(intent);
                    Finish();
                }
                else
                {
                    Toast.MakeText(this, "Account creation failed. Please try again.", ToastLength.Long).Show();
                    signInActive = false;
                }
            }
        }

        /// <summary>
        /// This method will try to log the user into Firebase
        /// </summary>
        /// <param name="account"></param>
        private void LoginWithFirebase(GoogleSignInAccount account)
        {
            var credentials = GoogleAuthProvider.GetCredential(account.IdToken, null);
            fireAuth.SignInWithCredential(credentials)
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this);
        }

        /// <summary>
        /// This method will be called if the sign in with Firebase is a success
        /// </summary>
        /// <param name="result"></param>
        public async void OnSuccess(Java.Lang.Object result)
        {
            Intent intent;

            // Check if they haven't made an account
            if(!await _api.CheckMadeAccountAsync(fireAuth))
            {
                if (!_api.IsConnectedToServer)
                {
                    Toast.MakeText(this, "Could not connect to the server. Please try again.", ToastLength.Long).Show();
                    fireAuth.SignOut();
                    signInActive = false;
                }
                else
                {
                    // They have not made an account. Go to the username activity
                    intent = new Intent(this, typeof(CreateUsernameActivity));
                    StartActivityForResult(intent, 2);
                }
                return;
            }

            Toast.MakeText(this, "Login successful", ToastLength.Short).Show();
            intent = new Intent(this, typeof(MainPageActivity));
            StartActivity(intent);
            Finish();
        }

        /// <summary>
        /// This method will be called if the sign in with Firebase does not work
        /// </summary>
        /// <param name="e"></param>
        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(this, "Login failed", ToastLength.Short).Show();
            signInActive = false;
        }
    }
}