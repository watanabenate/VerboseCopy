using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using Verbose.API;

namespace Verbose
{
    [Activity(Label = "CreateUsernameActivity")]
    public class CreateUsernameActivity : Activity
    {
        private VerboseAPIService _api;

        private Button submitButton;
        private EditText usernameTextBox;
        private TextView errorMessage;

        private string _userNameEntry;

        private bool userNameCheckActive;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.username_entry);

            // Get the api
            _api = VerboseAPIService.Instance;

            submitButton = FindViewById<Button>(Resource.Id.username_submit);
            submitButton.Click += Username_Enter_Click;

            usernameTextBox = FindViewById<EditText>(Resource.Id.username_field);

            errorMessage = FindViewById<TextView>(Resource.Id.username_error);

            _userNameEntry = "";
            userNameCheckActive = false;
        }

        private async void Username_Enter_Click(object sender, EventArgs e)
        {
            if (!userNameCheckActive)
            {
                userNameCheckActive = true;

                // Don't execute if they haven't changed the name
                string tempNewName = usernameTextBox.Text.Trim();
                if (_userNameEntry == tempNewName) { return; }

                _userNameEntry = tempNewName;

                bool result = false;
                result = await _api.CheckUsernameAvailableAsync(_userNameEntry); // Contact the server
                if (result)
                {
                    userNameCheckActive = false;
                    Intent intent = new Intent(this, typeof(LoginScreenActivity));
                    intent.PutExtra("username", _userNameEntry);
                    SetResult(Result.Ok, intent);
                    Finish();
                    return;
                }

                // Else display error and try again
                errorMessage.Text = "Username is not available. Please try again.";

                errorMessage.Visibility = ViewStates.Visible;
                userNameCheckActive = false;
            }
        }
    }
}