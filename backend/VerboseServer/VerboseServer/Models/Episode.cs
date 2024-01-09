using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VerboseServer.Models
{
    public class Episode
    {
        public Episode() {
            LikedBy = new HashSet<LikedByEpisode>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int EpisodeID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Creator { get; set; }
        public string PlayLink { get; set; }
        public string? CoverArtLink { get; set; }
        public virtual List<Comment> Comments { get; set; } = new List<Comment>();
        public virtual HashSet<LikedByEpisode> LikedBy { get; set; }
        public int LikeCount { get; set; }
        public int? PodchaserPodcastID { get; set; }
        //public int PodchaserID { get; set; } 
    }
}
