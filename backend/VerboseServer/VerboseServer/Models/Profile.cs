namespace VerboseServer.Models
{
    public class Profile
    {
       
        public int ProfileID { get; set; }
        public string Name { get; set; }

        public bool IsCreator { get; set; } = false;
        public string? RssUrl { get; set; }

        public string Email { get; set; }
        // list of recently listened to podcast episodes
        public List<ListenedTo>? RecentlyListenedTo { get; set; }

        public PublicProfile PublicProfileInfo { get; set; }
        public int? PublicProfileID { get; set; }
    }
}
