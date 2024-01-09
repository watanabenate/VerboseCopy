namespace VerboseServer.Models
{
    public class ProfileApp
    {
        public ProfileApp(Profile profile, PublicProfileApp publicProfile)
        {
            ProfileID = profile.ProfileID;
            Name = profile.Name;
            IsCreator = profile.IsCreator;
            RssUrl = profile.RssUrl;
            Email = profile.Email;
            RecentlyListenedTo = profile.RecentlyListenedTo;
            PublicProfileID = publicProfile.PublicProfileID;
            PublicProfileInfo = publicProfile;
        }

       
        public int ProfileID { get; set; }
        public string Name { get; set; }

        public bool IsCreator { get; set; } = false;
        public string? RssUrl { get; set; }

        public string Email { get; set; }
        // list of recently listened to podcast episodes
        public List<ListenedTo>? RecentlyListenedTo { get; set; }

        public PublicProfileApp PublicProfileInfo { get; set; }
        public int? PublicProfileID { get; set; }
    }
}
