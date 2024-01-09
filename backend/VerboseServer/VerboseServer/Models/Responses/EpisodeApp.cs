using System.ComponentModel.DataAnnotations.Schema;

namespace VerboseServer.Models
{
    public class EpisodeApp
    {
        public EpisodeApp()
        {
            LikedBy = new HashSet<int>();
        }

        public EpisodeApp(Episode e, List<CommentApp> c)
        {
            EpisodeID = e.EpisodeID;
            Title = e?.Title;
            Description = e?.Description;
            Creator = e?.Creator;
            PlayLink = e.PlayLink;
            CoverArtLink = e?.CoverArtLink;
            LikeCount = e.LikeCount;
            PodchaserPodcastID = e?.PodchaserPodcastID;

            LikedBy = new HashSet<int>();
            foreach(LikedByEpisode lb in e.LikedBy)
            {
                LikedBy.Add(lb.PublicProfileID);
            }

            Comments = c;
        }

        public int EpisodeID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Creator { get; set; }
        public string PlayLink { get; set; }
        public string? CoverArtLink { get; set; }
        public virtual List<CommentApp> Comments { get; set; } = new List<CommentApp>();
        public virtual HashSet<int> LikedBy { get; set; }
        public int LikeCount { get; set; }
        public int? PodchaserPodcastID { get; set; }
        //public int PodchaserID { get; set; } 
    }
}
