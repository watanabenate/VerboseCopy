using System.ComponentModel;

namespace Verbose.Data
{
    public interface IPodcastEpisode : INotifyPropertyChanged
    {
        string Title { get; set; } // The title for this episode
        string Creator { get; set; } // The creator name
        string PlayLink { get; set; } // Where the audio link is
        string CoverArtLink { get; set; } // Art for this episode
    }
}
