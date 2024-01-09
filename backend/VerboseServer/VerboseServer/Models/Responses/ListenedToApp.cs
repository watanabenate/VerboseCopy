namespace VerboseServer.Models.Responses
{
    public class ListenedToApp
    {
        public ListenedToApp(EpisodeApp e, long timestamp)
        {
            Episode = e;
            Timestamp = timestamp;
        }

        public EpisodeApp Episode { get; set; }
        public long Timestamp { get; set; } = 0;
    }
}
