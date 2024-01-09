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
    internal class EpisodeInfoCardAdapter : RecyclerView.Adapter
    {
        public override int ItemCount => mEpisodeList.Count;

        public List<PodcastEpisode> mEpisodeList;
        VerboseAPIService _api;

        public EpisodeInfoCardAdapter(List<PodcastEpisode> episodeList)
        {
            mEpisodeList = episodeList;
            _api = VerboseAPIService.Instance;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.episode_card, parent, false);

            return new EpisodeInfoViewHolder(itemView, OnClick);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            EpisodeInfoViewHolder vh = holder as EpisodeInfoViewHolder;
            vh.CoverArt.SetImageBitmap(_api.GetImageBitmap(mEpisodeList[position]));
            vh.Title.Text = mEpisodeList[position].Title;
            if(mEpisodeList[position].Description != null)
            {
                vh.Description.Text = mEpisodeList[position].Description;
            }
        }

        public event EventHandler<int> ItemClick;
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}