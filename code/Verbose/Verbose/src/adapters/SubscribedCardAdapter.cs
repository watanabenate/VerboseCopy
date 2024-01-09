using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verbose.Data;
using Verbose.API;
using Verbose.src.viewholders;
using Android.Graphics;

namespace Verbose.src.adapters
{
    public class SubscribedCardAdapter : RecyclerView.Adapter
    {
        public override int ItemCount => subscribedPodcastList.Count;

        public List<Podcast> subscribedPodcastList;

        private VerboseAPIService _api;

        public bool ShowUnsubscribeButton = true;

        public bool OnOtherUserPage;

        public SubscribedCardAdapter()
        {
            subscribedPodcastList = new List<Podcast>();
            _api = VerboseAPIService.Instance;
            OnOtherUserPage = false;
        }

        public SubscribedCardAdapter(bool OnOtherUserPage) : this()
        {
            this.OnOtherUserPage = OnOtherUserPage;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.subscribed_card, parent, false);

            return new SubscribedViewHolder(itemView, OnUnsubscribeSubscribeClick, OnPodcastClick);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            SubscribedViewHolder vh = holder as SubscribedViewHolder;

            vh.podcastImage.SetImageBitmap(_api.GetImageBitmap(subscribedPodcastList[position]));

            if (!OnOtherUserPage)
            {
                vh.unsubscribeSubscribeButton.Text = "Unsubscribe";
            }
            else
            {
                vh.unsubscribeSubscribeButton.Text = _api.UserProfile.PublicProfileInfo.Subscribed.Contains(subscribedPodcastList[position]) ? "Unsubscribe" : "Subscribe";
            }

            vh.podcastName.Text = subscribedPodcastList[position].Title;

            if(ShowUnsubscribeButton)
            {
                vh.unsubscribeSubscribeButton.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.unsubscribeSubscribeButton.Visibility = ViewStates.Gone;
            }
        }

        public event EventHandler<int> UnsubscribeSubscribeClick;
        void OnUnsubscribeSubscribeClick(int position)
        {
            if (UnsubscribeSubscribeClick != null)
                UnsubscribeSubscribeClick(this, position);
        }

        public event EventHandler<int> PodcastClick;
        void OnPodcastClick(int position)
        {
            if (PodcastClick != null)
                PodcastClick(this, position);
        }
    }
}