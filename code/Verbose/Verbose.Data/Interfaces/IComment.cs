using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Verbose.Data
{
    public interface IComment : INotifyPropertyChanged
    {
        public string Text { get; set; }
        public int Likes { get; set; }
        public DateTime Date { get; set; }
        public long Timestamp { get; set; }
        public List<IComment> Replies { get; set; }
    }
}