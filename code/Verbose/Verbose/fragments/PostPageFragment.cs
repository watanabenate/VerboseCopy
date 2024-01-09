using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Java.Interop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Verbose.API;
using Verbose.Data;
using Verbose.fragments;
using Verbose.src.adapters;

namespace Verbose
{
    public class PostPageFragment : AndroidX.Fragment.App.Fragment
    {
        VerboseAPIService _api;

        private bool searchActive = false;

        View view;

        RecyclerView commentsRecycler;
        CommentCardAdapter commentsAdapter;
        RecyclerView.LayoutManager mLayoutManager;

        Post post;

        ImageButton posterPicture;
        TextView posterUsername;
        ImageView postImage;
        TextView postTitle;
        TextView postDescription;

        ImageButton likeButton;
        TextView likeCount;

        TextView commentCount;
        ImageButton commentSubmit;
        EditText commentText;

        bool isLiked;
        bool likeActive;
        bool commentActive;


        /// <summary>
        /// This is called whenever the fragment is made.
        /// 
        /// This page has two views: Search and SearchResults
        /// </summary>
        /// <param name="inflater"></param>
        /// <param name="container"></param>
        /// <param name="savedInstanceState"></param>
        /// <returns></returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            container.RemoveAllViews();
            view = inflater.Inflate(Resource.Layout.post_view_page, container, false);

            // add back button
            view.FindViewById<ImageButton>(Resource.Id.min_post_view).Click += (o, s) => ((MainPageActivity)Activity).OnBackPressed();

            base.OnCreate(savedInstanceState);

            savedInstanceState = this.Arguments;
            post = ((PostParcelable) savedInstanceState.GetParcelable("post")).p;


            // Get the api
            _api = VerboseAPIService.Instance;

            posterPicture = view.FindViewById<ImageButton>(Resource.Id.profile_image);
            if(post.ProfileImageLink != null && post.ProfileImageLink != "")
            {
                posterPicture.SetImageBitmap(_api.GetImageBitmap(post.ProfileImageLink));
            }
            posterPicture.Click += GoToProfile;

            posterUsername = view.FindViewById<TextView>(Resource.Id.username);
            posterUsername.Text = post.Username;
            posterUsername.Click += GoToProfile;

            if(post.ImageURL != null && post.ImageURL != "")
            {
                postImage = view.FindViewById<ImageView>(Resource.Id.podcast);
                postImage.SetImageBitmap(_api.GetImageBitmap(post.ImageURL));
                postImage.Visibility = ViewStates.Visible;
                postImage.Click += GoToPostPodcast;
            }

            postTitle = view.FindViewById<TextView>(Resource.Id.post_title);
            postTitle.Text = post.Title;

            postDescription = view.FindViewById<TextView>(Resource.Id.post_content);
            postDescription.Text = post.Description;

            likeButton = view.FindViewById<ImageButton>(Resource.Id.like);
            likeButton.Click += LikePost;
            LikedBy userLike = new LikedBy()
            {
                PostID = post.PostID,
                PublicProfileID = _api.UserProfile.PublicProfileInfo.PublicProfileId,
            };
            isLiked = false;
            if(post.LikedBy.Contains(userLike))
            {
                likeButton.SetImageResource(Resource.Drawable.filled_like);
                isLiked = true;
            }

            likeCount = view.FindViewById<TextView>(Resource.Id.like_count);
            likeCount.Text = post.LikedBy.Count.ToString();

            commentCount = view.FindViewById<TextView>(Resource.Id.comment_count);
            commentCount.Text = post.Comments.Count.ToString();

            commentText = view.FindViewById<EditText>(Resource.Id.comment_text_input);
            commentSubmit = view.FindViewById<ImageButton>(Resource.Id.comment_text_submit);
            commentSubmit.Click += SubmitComment;

            commentsRecycler = view.FindViewById<RecyclerView>(Resource.Id.post_comments_recycler);
            mLayoutManager = new LinearLayoutManager(Context);
            commentsRecycler.SetLayoutManager(mLayoutManager);

            commentsAdapter = new CommentCardAdapter(post.Comments, false);
            commentsAdapter.ProfileClickHandler += GoToProfileComment;

            commentsRecycler.SetAdapter(commentsAdapter);

            return view;
        }

        private async void SubmitComment(object sender, EventArgs e)
        {
            // Check if they have anything yet
            if (commentText.Text.Length <= 0)
            {
                Toast.MakeText(Context, "Please submit some text before submitting a comment.", ToastLength.Short).Show();
                return;
            }
            // Cannot be over a certain length
            else if (commentText.Text.Length > 200)
            {
                Toast.MakeText(Context, "Text is too long (needs to be under 200 characters).", ToastLength.Short).Show();
                return;
            }

            // This is so we don't keep sending to the server the same comment
            if (commentActive) { return; }
            commentActive = true;

            // Send to the server
            Comment c = new Comment
            {
                Text = commentText.Text,
                Likes = 0,
                Date = DateTime.Now,
                Username = _api.UserProfile.PublicProfileInfo.UserName,
                ProfileImageLink = _api.UserProfile.PublicProfileInfo.PictureLink,
                ProfileID = _api.UserProfile.PublicProfileInfo.PublicProfileId,
            };

            if (await _api.SubmitComment(c, post))
            {
                // Add to the recylcer view
                post.Comments.Insert(0, c);
                commentsAdapter.NotifyItemInserted(0);
                commentCount.Text = post.Comments.Count.ToString();
                

                Toast.MakeText(Context, "Comment submitted successfully.", ToastLength.Short).Show();
                commentText.Text = "";
            }
            // Failed
            else
            {
                Toast.MakeText(Context, "Comment submission failed.", ToastLength.Short).Show();
            }

            commentActive = false;
        }

        private async void LikePost(object sender, EventArgs e)
        {
            if (!likeActive)
            {
                likeActive = true;
                // Check if this podcast is liked or not
                if (!isLiked)
                {
                    // Set it to liked and contact server. If we couldn't contact or if it failed, unlike
                    switchLikeButton();

                    if (!await _api.LikeOrUnlikePost(post, true))
                    {
                        switchLikeButton();
                        Toast.MakeText(Context, "Error on the server with liking", ToastLength.Short).Show();
                    }
                }
                else
                {
                    switchLikeButton();

                    if (!await _api.LikeOrUnlikePost(post, false))
                    {
                        switchLikeButton();
                        Toast.MakeText(Context, "Error on the server with un-liking", ToastLength.Short).Show();
                    }

                }
                likeActive = false;
            }
        }

        void switchLikeButton()
        {
            LikedBy userLikedBy = new LikedBy()
            {
                PostID = post.PostID,
                PublicProfileID = _api.UserProfile.PublicProfileInfo.PublicProfileId,
            };

            if (isLiked)
            {
                isLiked = false;
                likeButton.SetImageResource(Resource.Drawable.empty_like);
                post.Likes -= 1;
                likeCount.Text = post.Likes.ToString();
                post.LikedBy.Remove(userLikedBy);
            }
            else
            {
                isLiked = true;
                likeButton.SetImageResource(Resource.Drawable.filled_like);
                post.Likes += 1;
                likeCount.Text = post.Likes.ToString();
                post.LikedBy.Add(userLikedBy);
            }
        }

        private async void GoToPostPodcast(object sender, EventArgs e)
        {
            if (await _api.GetPodcastFromId(post.Episode.PodchaserPodcastID))
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
            }
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

        private async void GoToProfile(object sender, EventArgs e)
        {
            // Make sure to have a check if it's the same user or a different user
            // Don't want to navigate to other profile page if it's the current user
            if (post.Username == _api.UserProfile.PublicProfileInfo.UserName)
            {
                ProfilePageFragment profileFragment = new ProfilePageFragment();
                ((MainPageActivity)Activity).ChangeFragment(profileFragment);
            }
            else
            {
                // Get the other user first
                if (await _api.GetOtherUserPublicProfile(post.Username))
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
    }
}
