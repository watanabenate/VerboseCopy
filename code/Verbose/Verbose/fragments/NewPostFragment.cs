using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verbose.API;
using Verbose.Data;

namespace Verbose.fragments
{
    public class NewPostFragment : AndroidX.Fragment.App.Fragment
    {
        VerboseAPIService _api;
        View view;

        EditText titleText;
        EditText bodyText;

        TextView errorText;

        Button submitButton;
        bool submitActive = false;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            container.RemoveAllViews();
            // Set our view from the "main" layout resource
            view = inflater.Inflate(Resource.Layout.new_post_page, container, false);

            // Get the api
            _api = VerboseAPIService.Instance;

            titleText = view.FindViewById<EditText>(Resource.Id.post_title_entry);

            bodyText = view.FindViewById<EditText>(Resource.Id.post_content_entry);

            submitButton = view.FindViewById<Button>(Resource.Id.post_submit_button);
            submitButton.Click += submitPost;

            errorText = view.FindViewById<TextView>(Resource.Id.post_error_text);

            // Add back button
            view.FindViewById<ImageButton>(Resource.Id.min_new_post).Click += (o, s) => ((MainPageActivity)Activity).OnBackPressed();

            return view;
        }

        private async void submitPost(object sender, EventArgs e)
        {
            if (titleText.Text.Length == 0)
            {
                errorText.Text = "You must have a title";
                return;
            }
            if (bodyText.Text.Length == 0)
            {
                errorText.Text = "You must have a post body";
                return;
            }
            if (titleText.Text.Length > 40)
            {
                errorText.Text = "The title is too long (Max 40 chars)";
                return;
            }
            if (titleText.Text.Length > 200)
            {
                errorText.Text = "The body is too long (Max 200 chars)";
                return;
            }

            if (submitActive)
            {
                return;
            }

            submitActive = true;

            Post p = new Post
            {
                Title = titleText.Text,
                Description = bodyText.Text,
                Username = _api.UserProfile.PublicProfileInfo.UserName,
                ProfileImageLink = _api.UserProfile.PublicProfileInfo.PictureLink,
                ProfileID = _api.UserProfile.PublicProfileInfo.PublicProfileId,
            };

            if (await _api.SubmitPost(p))
            {
                _api.UserProfile.PublicProfileInfo.Posts.Insert(0, p);
                Toast.MakeText(Context, "Post submitted successfully", ToastLength.Short).Show();
                ((MainPageActivity)Activity).OnBackPressed();
            }
            else
            {
                errorText.Text = "Post could not be submitted. Please try again.";
            }

            submitActive = false;
        }
    }
}