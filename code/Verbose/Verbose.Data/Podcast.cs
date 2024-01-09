using Android.Graphics;
using System.Collections.Generic;
using System.ComponentModel;

namespace Verbose.Data
{
    public class Podcast : IPodcast
    {
        #region IPodcast Implementation

        private int _podcastID;
        private List<PodcastEpisode> _episodes;
        private string _creator = "";
        private string _coverArtLink = "";
        private string _title = "";
        private string _description = "";

        public Bitmap CoverArtImage;

        public int PodcastID
        {
            get
            {
                return _podcastID;
            }
            set
            {
                if (_podcastID != value)
                {
                    _podcastID = value;
                    NotifyPropertyChanged("PodcastID");
                }
            }
        }

        public List<PodcastEpisode> Episodes { 
            get
            {
                return _episodes;
            }
            set
            {
                if(_episodes != value)
                {
                    _episodes = value;
                    NotifyPropertyChanged("Episodes");
                }
            }
        }
        public string Creator 
        { 
            get { return _creator; }
            set
            {
                if(_creator != value)
                {
                    _creator = value;
                    NotifyPropertyChanged("Creator");
                }
            }
        }
        public string CoverArtLink
        {
            get { return _coverArtLink; }
            set
            {
                if(_coverArtLink != value)
                {
                    _coverArtLink = value;
                    NotifyPropertyChanged("CoverArtLink");
                }
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }



        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// INotifyPropertyChanged Raise PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of property that changed</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public override int GetHashCode()
        {
            return PodcastID * Title.GetHashCode() * 31;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            Podcast p = (Podcast)obj;

            if (p.Title == this.Title
                && p.PodcastID == this.PodcastID)
                return true;

            return false;
        }
        #endregion
    }
}
