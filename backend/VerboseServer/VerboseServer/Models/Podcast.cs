using System.ComponentModel.DataAnnotations.Schema;

namespace VerboseServer.Models
{
    public class Podcast
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PodcastID { get; set; }
        public string? Creator { get; set; }
        public string? CoverArtLink { get; set; }
        public List<Episode>? Episodes { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<PodcastCategory>? Categories { get; set; }
        //public int PodchaserID { get; set; }
    }

    public class PodcastCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PodcastCategoryID { get; set; }
        public string topic { get; set; }

        
    }
}
