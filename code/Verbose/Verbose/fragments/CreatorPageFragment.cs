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
    public class CreatorPageFragment : AndroidX.Fragment.App.Fragment
    {
        VerboseAPIService _api;

        View view;

        EditText rssLinkInput;
        Button submitButton;

        private bool rssBtnActive = false;

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
            view = inflater.Inflate(Resource.Layout.creator_verification_page, container, false);

            // Add back button
            view.FindViewById<ImageButton>(Resource.Id.min_creator).Click += (o, s) => ((MainPageActivity)Activity).OnBackPressed();

            base.OnCreate(savedInstanceState);

            // Get the api
            _api = VerboseAPIService.Instance;

            rssLinkInput = view.FindViewById<EditText>(Resource.Id.rss_link_input);
            submitButton = view.FindViewById<Button>(Resource.Id.submit_rss_btn);
            submitButton.Click += SubmitRssLink;

            return view;
        }

        private async void SubmitRssLink(object sender, EventArgs e)
        {
            if (rssBtnActive) return;

            rssBtnActive = true;

            if(rssLinkInput.Text.Length == 0)
            {
                Toast.MakeText(Context, "Must have a valid link.", ToastLength.Short).Show();

                rssBtnActive=false;
                return;
            }

            if(await _api.SubmitRssLink(rssLinkInput.Text))
            {
                Toast.MakeText(Context, "Successfully verified as a creator!", ToastLength.Long).Show();
                ((MainPageActivity)Activity).OnBackPressed();
            }
            else
            {
                Toast.MakeText(Context, "Could not verify you as a creator. Please check the link and make sure your email is the same as on the RSS Link.", ToastLength.Long).Show();
            }
            rssBtnActive = false;
        }
    }
}