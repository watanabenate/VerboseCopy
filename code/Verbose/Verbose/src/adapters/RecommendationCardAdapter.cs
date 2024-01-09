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
using Verbose.API;
using Verbose.Data;
using Verbose.src.viewholders;

namespace Verbose.src.adapters
{
    internal class RecommendationCardAdapter : RecyclerView.Adapter
    {
        public override int ItemCount => podcastList.Count;

        public List<Podcast> podcastList;
        VerboseAPIService _api;

        public RecommendationCardAdapter(List<Podcast> podcastList)
        {
            this.podcastList = podcastList;
            _api = VerboseAPIService.Instance;
        }

        public RecommendationCardAdapter()
        {
            podcastList = new List<Podcast>();
            _api = VerboseAPIService.Instance;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.podcast_card, parent, false);

            return new EpisodeViewHolder(itemView, OnClick);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            EpisodeViewHolder vh = holder as EpisodeViewHolder;
            vh.CoverArt.SetImageBitmap(_api.GetImageBitmap(podcastList[position]));
            vh.EpisodeTitle.Text = podcastList[position].Title;
        }

        public event EventHandler<int> ItemClick;
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}

    