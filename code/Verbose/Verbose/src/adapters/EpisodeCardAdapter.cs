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
    internal class EpisodeCardAdapter : RecyclerView.Adapter
    {
        public override int ItemCount => episodeList.Count;

        public List<ListenedTo> episodeList;
        VerboseAPIService _api;

        public EpisodeCardAdapter(List<ListenedTo> episodeList)
        {
            this.episodeList = episodeList;
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
            vh.CoverArt.SetImageBitmap(_api.GetImageBitmap(episodeList[position].Episode));
            vh.EpisodeTitle.Text = episodeList[position].Episode.Title;
        }

        public event EventHandler<int> ItemClick;
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}

    