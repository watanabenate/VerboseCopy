using Android.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Verbose.Data
{
    public class PodcastEpisode : IPodcastEpisode
    {
        #region IPodcastEpisode Implementation
        private string _title;
        private string _creator;
        private string _playLink;
        private string _coverArtLink;
        private List<Comment> _comments;
        private HashSet<int> _likedBy;
        private int _likeCount;
        private string _description;
        public int EpisodeID { get; set; }
        private int _podchaserPodcastID { get; set; }

        public Bitmap CoverArtImage;

        public PodcastEpisode()
        {
            Comments = new List<Comment>();
            LikedBy = new HashSet<int>();
            Description = "";
        }

        public int PodchaserPodcastID
        {
            get { return _podchaserPodcastID; }
            set
            {
                if (_podchaserPodcastID != value)
                {
                    _podchaserPodcastID = value;
                    NotifyPropertyChanged("PodcastId");
                }
            }
        }

        public int LikeCount
        {
            get { return _likeCount; }
            set
            {
                if (_likeCount != value)
                {
                    _likeCount = value;
                    NotifyPropertyChanged("LikeCount");
                }
            }
        }

        public HashSet<int> LikedBy
        {
            get { return _likedBy; }
            set
            {
                if (_likedBy != value)
                {
                    _likedBy = value;
                    NotifyPropertyChanged("LikedBy");
                }
            }
        }

        public List<Comment> Comments
        {
            get { return _comments; }
            set
            {
                if (_comments != value)
                {
                    _comments = value;
                    NotifyPropertyChanged("Comments");
                }
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                if(_title != value)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
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
        public string PlayLink
        {
            get { return _playLink; }
            set
            {
                if(_playLink != value)
                {
                    _playLink = value;
                    NotifyPropertyChanged("PlayLink");
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
        public string Description
        {
            get { return _description; }
            set
            {
                if(_description != value)
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

        #endregion
    }
}
