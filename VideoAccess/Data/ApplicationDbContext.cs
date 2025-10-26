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

        public DbSet<VideoEntity> Videos { get; set; } = null!;

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

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.UpdatedAt)
                    .IsRequired(false);

                entity.Property(e => e.FileSize)
                    .IsRequired();

                entity.Property(e => e.ProcessingStatus)
                    .IsRequired()
                    .HasConversion<int>(); // Enum хранится как int

                entity.Property(e => e.ErrorMessage)
                    .HasMaxLength(1000)
                    .IsRequired(false);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.FileName);
                entity.HasIndex(e => e.ProcessingStatus);
            });
        }
    }
}
