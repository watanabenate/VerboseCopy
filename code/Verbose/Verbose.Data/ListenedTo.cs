using System.Collections.Generic;
using System.ComponentModel;

namespace Verbose.Data
{
    public class ListenedTo
    {
        private PodcastEpisode _episode;
        private long _timestamp;

        public ListenedTo()
        {

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

        public long Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (_timestamp != value)
                {
                    _timestamp = value;
                    NotifyPropertyChanged("Timestamp");
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