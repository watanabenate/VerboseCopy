using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verbose.API;

namespace Verbose.fragments
{
    public class NewProfilePictureFragment : AndroidX.Fragment.App.Fragment
    {
        View view; 
        VerboseAPIService _api;
        ImageButton img1, img2, img3, img4, img5, googImg;
        ImageView googPhoto; 
        string imgLink1, imgLink2, imgLink3, imgLink4, imgLink5, imgLink6;
        ImageView profPic; 


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            container.RemoveAllViews();
            view = inflater.Inflate(Resource.Layout.profile_picture_page, container, false);
            base.OnCreate(savedInstanceState);

            
            _api = VerboseAPIService.Instance;
            googPhoto = view.FindViewById<ImageView>(Resource.Id.profile_image6); 
            if (_api.UserProfile.PublicProfileInfo.PictureLink != "" && _api.UserProfile.PublicProfileInfo.PictureLink != null)
            {
                googPhoto.SetImageBitmap(_api.GetImageBitmap(_api.UserProfile.PublicProfileInfo.GooglePictureLink));
            }
            SetupPage();
            return view; 

        }

        private async void SetupPage()
        {
            //TODO: ADD SETUP FOR ALL IMG BUTTONS \
            img1 = view.FindViewById<ImageButton>(Resource.Id.profile_image1);
            img1.Click += Img1Click;

            img2 = view.FindViewById<ImageButton>(Resource.Id.profile_image2);
            img2.Click += Img2Click;

            img3 = view.FindViewById<ImageButton>(Resource.Id.profile_image3);
            img3.Click += Img3Click;

            img4 = view.FindViewById<ImageButton>(Resource.Id.profile_image4);
            img4.Click += Img4Click;

            img5 = view.FindViewById<ImageButton>(Resource.Id.profile_image5);
            img5.Click += Img5Click;

            googImg = view.FindViewById<ImageButton>(Resource.Id.profile_image6); 
            googImg.Click += Img6Click; 

            imgLink1 = "https://images.unsplash.com/photo-1531876137992-22b6ce5221f1?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2730&q=80";
            imgLink2 = "https://images.unsplash.com/photo-1465146344425-f00d5f5c8f07?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1176&q=80";
            imgLink3 = "https://images.unsplash.com/photo-1497449493050-aad1e7cad165?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=765&q=80";
            imgLink4 = "https://images.unsplash.com/photo-1527489377706-5bf97e608852?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=959&q=80";
            imgLink5 = "https://images.unsplash.com/photo-1532274402911-5a369e4c4bb5?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1170&q=80";
            imgLink6 = _api.UserProfile.PublicProfileInfo.GooglePictureLink; 
        }

        private async void Img1Click(object sender, EventArgs e)
        {
            await _api.UpdateProfilePictureAsync(imgLink1); 
            ProfilePageFragment profileFragment = new ProfilePageFragment();
            ((MainPageActivity)Activity).ChangeFragment(profileFragment); 
        }

        private async void Img2Click(object sender, EventArgs e)
        {
            await _api.UpdateProfilePictureAsync(imgLink2);
            ProfilePageFragment profileFragment = new ProfilePageFragment();
            ((MainPageActivity)Activity).ChangeFragment(profileFragment);
        }

        private async void Img3Click(object sender, EventArgs e)
        {
            await _api.UpdateProfilePictureAsync(imgLink3);
            ProfilePageFragment profileFragment = new ProfilePageFragment();
            ((MainPageActivity)Activity).ChangeFragment(profileFragment);
        }

        private async void Img4Click(object sender, EventArgs e)
        {
            await _api.UpdateProfilePictureAsync(imgLink4);
            ProfilePageFragment profileFragment = new ProfilePageFragment();
            ((MainPageActivity)Activity).ChangeFragment(profileFragment);
        }

        private async void Img5Click(object sender, EventArgs e)
        {
            await _api.UpdateProfilePictureAsync(imgLink5);
            ProfilePageFragment profileFragment = new ProfilePageFragment();
            ((MainPageActivity)Activity).ChangeFragment(profileFragment);
        }
        private async void Img6Click(object sender, EventArgs e)
        {
            await _api.UpdateProfilePictureAsync(imgLink6);
            ProfilePageFragment profileFragment = new ProfilePageFragment();
            ((MainPageActivity)Activity).ChangeFragment(profileFragment);
        }
    }
}