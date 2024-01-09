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
using Verbose.fragments;
using Verbose.src.adapters;

namespace Verbose
{
    public class ProfilePageFragment : AndroidX.Fragment.App.Fragment
    {
        VerboseAPIService _api;

        RecyclerView postsListRecycler;

        RecyclerView.LayoutManager mLayoutManager;

        PostCardAdapter mPostAdapter;

        View view;
        ImageButton settingsBtn;
        Button friendBtn;
        Button subBtn;
        TextView usernameText;
        ImageView profilePhoto;
        TextView subsCountText;
        TextView friendsCountText;

        TextView postsPageCountText;

        ImageButton profPicBtn; 

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
            view = inflater.Inflate(Resource.Layout.user_posts_page, container, false);

            base.OnCreate(savedInstanceState);

            // Get the api
            _api = VerboseAPIService.Instance;

            RefreshUserPosts();

            SetupPage();

            postsListRecycler = view.FindViewById<RecyclerView>(Resource.Id.post_list_recycler);
            mLayoutManager = new LinearLayoutManager(Context);
            postsListRecycler.SetLayoutManager(mLayoutManager);

            mPostAdapter = new PostCardAdapter();
            mPostAdapter.PostClick += GoToPost;
            mPostAdapter.EpisodeClick += GoToPostPodcast;

            mPostAdapter.postList = _api.UserProfile.PublicProfileInfo.Posts;
            postsListRecycler.SetAdapter(mPostAdapter);

            postsPageCountText = view.FindViewById<TextView>(Resource.Id.posts_count);
            postsPageCountText.Text = _api.UserProfile.PublicProfileInfo?.Posts.Count.ToString();


            return view;
        }

        private async void RefreshUserPosts()
        {
            _api.UserProfile.PublicProfileInfo.Posts = new List<Post>();
            await _api.RefreshUserPosts();

            mPostAdapter.postList = _api.UserProfile.PublicProfileInfo.Posts;
            int size = mPostAdapter.ItemCount;
            mPostAdapter.NotifyDataSetChanged();
            postsPageCountText.Text = _api.UserProfile.PublicProfileInfo?.Posts.Count.ToString();
        }

        private void Logout(object sender, EventArgs e)
        {
            _api.logout();
            ((MainPageActivity)Activity).Logout();
        }

        private void SetupPage()
        {
            friendBtn = view.FindViewById<Button>(Resource.Id.friends);
            friendBtn.Click += FriendClick;

            subBtn = view.FindViewById<Button>(Resource.Id.subs);
            subBtn.Click += SubClick;

            settingsBtn = view.FindViewById<ImageButton>(Resource.Id.settings_btn);
            settingsBtn.Click += SettingsClick;

            usernameText = view.FindViewById<TextView>(Resource.Id.user);
            usernameText.Text = _api.UserProfile.PublicProfileInfo.UserName;

            profPicBtn = view.FindViewById<ImageButton>(Resource.Id.new_picture_button);
            profPicBtn.Click += PictureClick; 

            profilePhoto = view.FindViewById<ImageView>(Resource.Id.small_profile);

            if (_api.UserProfile.PublicProfileInfo.PictureLink != "" && _api.UserProfile.PublicProfileInfo.PictureLink != null)
            {
                profilePhoto.SetImageBitmap(_api.GetImageBitmap(_api.UserProfile.PublicProfileInfo.PictureLink));
            }

            subsCountText = view.FindViewById<TextView>(Resource.Id.subs_count);
            subsCountText.Text = _api.UserProfile.PublicProfileInfo?.Subscribed.Count.ToString();

            friendsCountText = view.FindViewById<TextView>(Resource.Id.friends_count);
            friendsCountText.Text = _api.UserProfile.PublicProfileInfo?.Following.Count.ToString();
        }

        private void PictureClick(object sender, EventArgs e)
        {
            NewProfilePictureFragment profilePictureFragment = new NewProfilePictureFragment();
            ((MainPageActivity)Activity).ChangeFragment(profilePictureFragment); 
        }

        private void SettingsClick(object sender, EventArgs e)
        {
            SettingsPageFragment settingsFragment = new SettingsPageFragment();

            // Send the url of the podcast
            ((MainPageActivity)Activity).ChangeFragment(settingsFragment, Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_slide_out_bottom);
        }

        private void FriendClick(object sender, EventArgs e)
        {
            ProfileFriendsPageFragment friendsFragment = new ProfileFriendsPageFragment();

            // Send the url of the podcast
            ((MainPageActivity)Activity).ChangeFragment(friendsFragment);
        }

        private void SubClick(object sender, EventArgs e)
        {
            ProfileSubsPageFragment subsFragment = new ProfileSubsPageFragment();

            // Send the url of the podcast
            ((MainPageActivity)Activity).ChangeFragment(subsFragment);
        }

        public void GoToPost(object sender, int position)
        {
            PostCardAdapter adapter = (PostCardAdapter)sender;
            Post post = adapter.postList[position];
            PostParcelable parcelable = new PostParcelable();
            parcelable.p = post;

            PostPageFragment postFragment = new PostPageFragment();
            Bundle bundle = new Bundle();
            bundle.PutParcelable("post", parcelable);

            postFragment.Arguments = bundle;

            // Send the url of the podcast
            ((MainPageActivity)Activity).ChangeFragment(postFragment);
        }

        private async void GoToPostPodcast(object sender, int position)
        {
            PostCardAdapter adapter = (PostCardAdapter)sender;
            Post post = adapter.postList[position];

            if (await _api.GetPodcastFromId(post.Episode.PodchaserPodcastID))
            {
                Podcast p = _api.PodcastFromId;

                Bundle b = new Bundle();
                b.PutParcelable("Podcast", new PodcastParcelable(p));

                // Get the other user first
                PodcastPageFragment podcastFragment = new PodcastPageFragment();
                podcastFragment.Arguments = b;
                ((MainPageActivity)Activity).ChangeFragment(podcastFragment);
            }
            else
            {
                Toast.MakeText(Context, "Could not load podcast", ToastLength.Short).Show();
            }

        }
    }
}