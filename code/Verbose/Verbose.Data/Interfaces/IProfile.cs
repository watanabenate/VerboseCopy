using System.Collections.Generic;
using System.ComponentModel;

namespace Verbose.Data
{
    public interface IProfile : INotifyPropertyChanged
    {
        PublicProfile PublicProfileInfo { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        bool IsLoggedIn { get; set; }
        bool IsCreator { get; set; }
        public List<ListenedTo> RecentlyListenedTo { get; set; }
    }
}
