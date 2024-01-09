using System.Collections.Generic;
using System.ComponentModel;

namespace Verbose.Data
{
    public interface IPublicProfile : INotifyPropertyChanged
    {
        string UserName { get; set; } // Username for this user
        string PictureLink { get; set; } // Link to their picture
        int PublicProfileId { get; set; } // Unique profile id
        //List<IPublicProfile> Following { get; set; } // People this user is following
        List<Podcast> Subscribed { get; set; } // Podcasts the user is subscribed to
        List<Post> Posts { get; set; }
    }
}
