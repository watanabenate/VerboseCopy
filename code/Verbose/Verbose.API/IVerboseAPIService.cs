using Android.Media;
using Firebase.Auth;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Verbose.Data;


namespace Verbose.API
{
    public interface IVerboseAPIService : INotifyPropertyChanged
    {
        bool IsConnected { get; } // If we are connected to the server

        List<ListenedTo> MainFeedRecentlyListenedTo { get; }

        Profile UserProfile { get; } // Current User's Profile

        MediaPlayer Player { get; }
        public FirebaseAuth fireAuth { get; set; }
        public void logout();

        Task<bool> GetMainFeedAsync(string username);
        Task<bool> CheckMadeAccountAsync(FirebaseAuth fireAuth);
        Task<bool> CreateUserAsync(FirebaseAuth fireAuth, string username);
        Task<bool> CheckUsernameAvailableAsync(string username);



    }
}
