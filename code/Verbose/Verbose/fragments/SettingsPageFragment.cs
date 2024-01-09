using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Verbose.API;
using Verbose.Data;
using Verbose.src.adapters;

namespace Verbose
{
    public class SettingsPageFragment : AndroidX.Fragment.App.Fragment
    {
        VerboseAPIService _api;

        // Settings Page
        Button logoutBtn;
        TextView creatorVerificationBtn, darkModeBtn, privacyBtn, sttBtn;

        View view;

        ProgressBar progressBar;
        private bool launchCaptionButtonActive;

        /// <summary>
        /// This is called whenever the fragment is made.
        /// </summary>
        /// <param name="inflater"></param>
        /// <param name="container"></param>
        /// <param name="savedInstanceState"></param>
        /// <returns></returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            container.RemoveAllViews();
            view = inflater.Inflate(Resource.Layout.settings_page, container, false);

            base.OnCreate(savedInstanceState);

            // Get the api
            _api = VerboseAPIService.Instance;

            launchCaptionButtonActive = false;

            logoutBtn = view.FindViewById<Button>(Resource.Id.logout_button);
            logoutBtn.Click += Logout;

            creatorVerificationBtn = view.FindViewById<TextView>(Resource.Id.creator_button);
            creatorVerificationBtn.Click += CreatorClick;

            darkModeBtn = view. FindViewById<TextView>(Resource.Id.dark_mode_button);
            darkModeBtn.Click += ToggleDarkMode;

            privacyBtn = view.FindViewById<TextView>(Resource.Id.privacy_button);
            if (_api.UserProfile.PublicProfileInfo.IsPublic)
            {
                privacyBtn.Text = GetString(Resource.String.privacy_private);
            }
            else
            {
                privacyBtn.Text = GetString(Resource.String.privacy_public);
            }

            privacyBtn.Click += PrivacyClick;

            sttBtn = view.FindViewById<TextView>(Resource.Id.stt_button);
            sttBtn.Click += LaunchCaptionDemo;

            progressBar = view.FindViewById<ProgressBar>(Resource.Id.settings_progress_bar);
            progressBar.Visibility = ViewStates.Gone;

            // Add back button
            view.FindViewById<ImageButton>(Resource.Id.min_settings).Click += (o, s) => ((MainPageActivity)Activity).OnBackPressed();

            return view;
        }

        private void CreatorClick(object sender, EventArgs e)
        {
            if(_api.UserProfile.IsCreator)
            {
                Toast.MakeText(Context, "You are already a creator", ToastLength.Short).Show();
                return;
            }

            CreatorPageFragment creatorFragment = new CreatorPageFragment();

            // Send the url of the podcast
            ((MainPageActivity)Activity).ChangeFragment(creatorFragment, Resource.Animation.slide_in_right, Resource.Animation.slide_out_right);
        }

        private void Logout(object sender, EventArgs e)
        {
            _api.logout();
            ((MainPageActivity)Activity).Logout();
        }

        private void ToggleDarkMode(object sender, EventArgs e)
        {
            ((MainPageActivity)Activity).ToggleDarkMode();
        }

        private async void PrivacyClick(object sender, EventArgs e)
        {
            string result = "";

            if(await _api.SetPrivacy(_api.UserProfile.PublicProfileInfo.PublicProfileId))
            {
                result = "Account Successfully Updated";
                
            }
            else
            {
                result = "An Error Occurred With The Server";
            }

            if (_api.UserProfile.PublicProfileInfo.IsPublic)
            {
                privacyBtn.Text = GetString(Resource.String.privacy_private);
            }
            else
            {
                privacyBtn.Text = GetString(Resource.String.privacy_public);
            }

            Toast.MakeText(Context, result, ToastLength.Short).Show();

        }

        private async void LaunchCaptionDemo(object sender, EventArgs e)
        {
            if(!launchCaptionButtonActive)
            {
                launchCaptionButtonActive = true;
                progressBar.Visibility = ViewStates.Visible;

                if (await _api.GetClosedCaptions())
                {
                    SttPlayerFragment sttFrag = new SttPlayerFragment();
                    ((MainPageActivity)Activity).ChangeFragment(sttFrag, Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_slide_out_bottom);
                    progressBar.Visibility = ViewStates.Gone;
                }
                else
                {
                    Toast.MakeText(Context, "An Error Occurred With The Server", ToastLength.Short).Show();
                    progressBar.Visibility = ViewStates.Gone;
                }

                launchCaptionButtonActive=false;
            }
        }
    }
}