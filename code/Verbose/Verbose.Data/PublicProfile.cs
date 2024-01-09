using System.Collections.Generic;
using System.ComponentModel;

namespace Verbose.Data
{
    public class PublicProfile : IPublicProfile
    {
        private string _userName;
        private string _pictureLink;
        private string _googlePictureLink; 
        private int _profileId;
        private bool _isPublic;
        private List<PublicProfile> _following;
        private List<Podcast> _subscribed;
        private List<Post> _posts;

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    NotifyPropertyChanged("UserName");
                }
            }
        }

        public string PictureLink
        {
            get { return _pictureLink; }
            set
            {
                if (_pictureLink != value)
                {
                    _pictureLink = value;
                    NotifyPropertyChanged("PictureLink");
                }
            }
        }

        public string GooglePictureLink
        {
            get { return _googlePictureLink; }
            set
            {
                if(_googlePictureLink != value)
                {
                    _googlePictureLink = value;
                    NotifyPropertyChanged("GooglePictureLink"); 
                }
            }
        }

        public int PublicProfileId
        {
            get { return _profileId; }
            set
            {
                if (_profileId != value)
                {
                    _profileId = value;
                    NotifyPropertyChanged("ProfileId");
                }
            }
        }

        public bool IsPublic
        {
            get { return _isPublic; }
            set
            {
                if(_isPublic != value)
                {
                    _isPublic = value;
                    NotifyPropertyChanged("IsPublic");
                }
            }
        }

        public List<PublicProfile> Following
        {
            get { return _following; }
            set
            {
                if (_following != value)
                {
                    _following = value;
                    NotifyPropertyChanged("Following");
                }
            }
        }

        public List<Podcast> Subscribed
        {
            get { return _subscribed; }
            set
            {
                if (_subscribed != value)
                {
                    _subscribed = value;
                    NotifyPropertyChanged("Subscribed");
                }
            }
        }

        public List<Post> Posts
        {
            get { return _posts; }
            set
            {
                if (_posts != value)
                {
                    _posts = value;
                    NotifyPropertyChanged("Posts");
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

        public override int GetHashCode()
        {
            return PublicProfileId * UserName.GetHashCode() * 31;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            PublicProfile p = (PublicProfile)obj;

            if (p.UserName == this.UserName
                && p.PublicProfileId == this.PublicProfileId)
                return true;

            return false;
        }
    }
}