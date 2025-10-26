using Microsoft.EntityFrameworkCore;
using VideoAccess.Models.Entities;

namespace VideoAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<VideoEntity> Videos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VideoEntity>(entity =>
            {
                entity.HasKey(e => e.VideoId);

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UploadedAt)
                    .IsRequired();

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.FileName);
            });
        }
    }
}
