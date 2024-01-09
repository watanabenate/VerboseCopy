using System.Collections.Generic;
using System.ComponentModel;

namespace Verbose.Data
{
    public class Profile : IProfile
    {
        private PublicProfile _publicProfileInfo;
        private string _name;
        private string _email;
        private bool _isLoggedIn;
        private bool _isCreator = false;
        private int _profileID;
        private List<ListenedTo> _recentlyListenedTo = new List<ListenedTo>();

        public Profile()
        {

        }

        public int ProfileID
        {
            get { return _profileID; }
            set
            {
                if (_profileID != value)
                {
                    _profileID = value;
                    NotifyPropertyChanged("ProfileID");
                }
            }
        }

        public PublicProfile PublicProfileInfo
        {
            get { return _publicProfileInfo; }
            set
            {
                if (_publicProfileInfo != value)
                {
                    _publicProfileInfo = value;
                    NotifyPropertyChanged("PublicProfileInfo");
                }
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    NotifyPropertyChanged("Email");
                }
            }
        }
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                if (_isLoggedIn != value)
                {
                    _isLoggedIn = value;
                    NotifyPropertyChanged("IsLoggedIn");
                }
            }
        }
        public bool IsCreator
        {
            get { return _isCreator; }
            set
            {
                if (_isCreator != value)
                {
                    _isCreator = value;
                    NotifyPropertyChanged("IsCreator");
                }
            }
        }
        public List<ListenedTo> RecentlyListenedTo
        {
            get { return _recentlyListenedTo; }
            set
            {
                if (_recentlyListenedTo != value)
                {
                    _recentlyListenedTo = value;
                    NotifyPropertyChanged("RecentlyListenedTo");
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