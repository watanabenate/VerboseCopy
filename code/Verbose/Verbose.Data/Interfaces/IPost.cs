using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Verbose.Data
{
    public interface IPost : INotifyPropertyChanged
    {
        public string ImageURL { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Comment> Comments { get; set; }
        public DateTime Date { get; set; }
        public int Likes { get; set; }
        public PodcastEpisode Episode { get; set; } // Each post connects to an episode
    }
}