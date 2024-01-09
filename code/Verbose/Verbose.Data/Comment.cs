using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Verbose.Data
{
    public class Comment : IComment
    {
        private string _text;
        private string _username;
        private string _imageLink;
        private int _profileID;
        private int _likes;
        private DateTime _date;
        private long _timestamp;
        private List<IComment> _replies;

        public Comment()
        {
            Likes = 0;
            Date = DateTime.Now;
            Replies = new List<IComment>();
            Text = "";
            Username = "";
            ProfileID = 0;
            ProfileImageLink = "";
            Timestamp = 0;
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    NotifyPropertyChanged("Text");
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

        public int Likes
        {
            get
            {
                return _likes;
            }
            set
            {
                if (_likes != value)
                {
                    _likes = value;
                    NotifyPropertyChanged("Likes");
                }
            }
        }

        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    NotifyPropertyChanged("Date");
                }
            }
        }

        public long Timestamp
        {
            get
            {
                return _timestamp;
            }
            set
            {
                if (_timestamp != value)
                {
                    _timestamp = value;
                    NotifyPropertyChanged("Timestamp");
                }
            }
        }

        public List<IComment> Replies
        {
            get
            {
                return _replies;
            }
            set
            {
                if (_replies != value)
                {
                    _replies = value;
                    NotifyPropertyChanged("Replies");
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