using Microsoft.EntityFrameworkCore;

namespace TaxAppealPlus.Models
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPost>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Excerpt).HasMaxLength(1000);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Author).HasMaxLength(200);
                entity.Property(e => e.FeaturedImage).HasMaxLength(1000);
                entity.HasIndex(e => e.Slug).IsUnique();
            });
        }
    }
}


