using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Verbose.Data
{
    public class Post : IPost
    {
        public int PostID { get; set; }
        private string? _imageURL;
        private string _title;
        private string _description;
        private List<Comment> _comments;
        private DateTime _date;
        private int _likes;
        private HashSet<LikedBy> _likedBy;
        private PodcastEpisode? _episode;
        private string _username;
        private string _imageLink;
        private int _profileID;

        public Post()
        {
            Comments = new List<Comment>();
            LikedBy = new HashSet<LikedBy>();
            Date = DateTime.Now;
            Likes = 0;
            Title = "";
            Description = "";
        }

        public string ImageURL
        {
            get { return _imageURL; }
            set
            {
                if (_imageURL != value)
                {
                    _imageURL = value;
                    NotifyPropertyChanged("ImageURL");
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
        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    NotifyPropertyChanged("Date");
                }
            }
        }
        public int Likes
        {
            get { return _likes; }
            set
            {
                if (_likes != value)
                {
                    _likes = value;
                    NotifyPropertyChanged("Likes");
                }
            }
        }
        public HashSet<LikedBy> LikedBy
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
        public PodcastEpisode Episode
        {
            get { return _episode; }
            set
            {
                if (_episode != value)
                {
                    _episode = value;
                    NotifyPropertyChanged("Episode");
                }
            }
        }
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                if (_username != value)
                {
                    _username = value;
                }
            }
        }

        public string ProfileImageLink
        {
            get
            {
                return _imageLink;
            }
            set
            {
                if (_imageLink != value)
                {
                    _imageLink = value;
                }
            }
        }

        public int ProfileID
        {
            get
            {
                return _profileID;
            }
            set
            {
                if (_profileID != value)
                {
                    _profileID = value;
                }
            }
        }

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