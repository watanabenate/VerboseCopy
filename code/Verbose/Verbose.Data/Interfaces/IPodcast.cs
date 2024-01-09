using System.Collections.Generic;
using System.ComponentModel;

namespace Verbose.Data
{
    public interface IPodcast : INotifyPropertyChanged
    {
        List<PodcastEpisode> Episodes { get; set; } // Episodes for this podcast
        string Creator { get; set; } // Name of the creator
        string CoverArtLink { get; set; } // Art for the creator
    }
}
