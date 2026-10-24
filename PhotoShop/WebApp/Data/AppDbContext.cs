using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Photo> Photo { get; set; }
        public DbSet<Photographer> Photographer { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<PhotoTag> PhotoTag { get; set; }
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<Download> Download { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PhotoTag>().HasKey(pt => new { pt.PhotoId, pt.TagId });
            modelBuilder.Entity<PhotoTag>().HasOne(pt => pt.Photo).WithMany(p => p.PhotoTags).HasForeignKey(pt => pt.PhotoId);
            modelBuilder.Entity<PhotoTag>().HasOne(pt => pt.Tag).WithMany(t => t.PhotoTags).HasForeignKey(pt => pt.TagId);
            modelBuilder.Entity<Download>().HasOne(d => d.Photo).WithMany(p => p.Downloads).HasForeignKey(d => d.PhotoId);
            modelBuilder.Entity<Download>().HasOne(d => d.AppUser).WithMany(u => u.Downloads).HasForeignKey(d => d.AppUserId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
