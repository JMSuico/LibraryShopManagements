using Microsoft.EntityFrameworkCore;
using LibraryShopManagement.Features.Data.Models;

namespace LibraryShopManagement.Features.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.Id);
                e.Property(u => u.FirstName).HasMaxLength(100);
                e.Property(u => u.LastName).HasMaxLength(100);
                e.Property(u => u.Address).HasMaxLength(300);
                e.Property(u => u.Username).IsRequired().HasMaxLength(100);
                e.Property(u => u.Email).IsRequired().HasMaxLength(200);
                e.Property(u => u.Password).IsRequired().HasMaxLength(255);
                e.Property(u => u.Role).HasConversion<string>();
                e.Property(u => u.ProfileImage).HasColumnType("LONGTEXT");
                e.Ignore(u => u.FullName); // computed, not stored
            });

            modelBuilder.Entity<Book>(e =>
            {
                e.HasKey(b => b.Id);
                e.Property(b => b.Title).IsRequired().HasMaxLength(200);
                e.Property(b => b.Author).IsRequired().HasMaxLength(150);
                e.Property(b => b.Price).HasColumnType("decimal(18,2)");
                e.Property(b => b.CoverImage).HasColumnType("LONGTEXT");
            });
        }
    }
}
