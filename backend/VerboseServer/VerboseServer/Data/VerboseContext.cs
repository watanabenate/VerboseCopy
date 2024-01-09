using Microsoft.EntityFrameworkCore;
using VerboseServer.Models;

namespace VerboseServer.Data
{
    public class VerboseContext : DbContext
    {
        public VerboseContext(DbContextOptions<VerboseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LikedBy>()
            .HasKey(b => b.LikedByID);

            modelBuilder.Entity<PodcastCategory>()
                .HasKey(b => b.PodcastCategoryID);
            modelBuilder.Entity<LikedByEpisode>()
                .HasKey(b => b.LikedByID);
        }
        
        public DbSet<Podcast> Podcasts { get; set; } = null!;
        public DbSet<Episode> Episodes { get; set; } = null!;
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<PublicProfile> PublicProfiles { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PodcastCategory> PodcastsCategories { get; set; }  
        public DbSet<VerboseServer.Models.Comment> Comments { get; set; }
        public DbSet<ListenedTo> ListenedTo { get; set; }
        public DbSet<VerboseServer.Models.CommentBy> CommentedBy { get; set; }

        // Weak Entities
        public DbSet<LikedBy> LikedBy { get; set; }
        public DbSet<LikedByEpisode> LikedByEpisode { get; set; }
        public DbSet<VerboseServer.Models.FollowedBy> FollowedBy { get; set;}
    }
}
