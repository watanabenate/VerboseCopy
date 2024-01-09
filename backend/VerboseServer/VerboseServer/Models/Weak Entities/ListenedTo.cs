namespace VerboseServer.Models
{
    public class ListenedTo
    {
        public int ListenedToID { get; set; }
        public int ProfileID { get; set; }
        public int EpisodeID { get; set; }
        public long Timestamp { get; set; }
        public DateTime DateListened { get; set; }

    }
}
