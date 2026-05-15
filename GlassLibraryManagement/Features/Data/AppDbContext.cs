using Microsoft.EntityFrameworkCore;
using GlassLibraryManagement.Features.Data.Models;

namespace GlassLibraryManagement.Features.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<QRCode> QRCodes { get; set; } = null!;
        public DbSet<BorrowTransaction> BorrowTransactions { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;

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
                e.Ignore(u => u.FullName);
                e.Ignore(u => u.BorrowLimit);
            });

            modelBuilder.Entity<Book>(e =>
            {
                e.HasKey(b => b.Id);
                e.Property(b => b.MainId).IsRequired().HasMaxLength(80);
                e.HasIndex(b => b.MainId).IsUnique();
                e.Property(b => b.Title).IsRequired().HasMaxLength(200);
                e.Property(b => b.Author).IsRequired().HasMaxLength(150);
                e.Property(b => b.TotalCopies);
                e.Property(b => b.AvailableCopies).HasColumnName("Quantity");
                e.Property(b => b.BorrowedCopies);
                e.Property(b => b.ReservedCopies);
                e.Property(b => b.Status).HasConversion<string>();
                e.Property(b => b.CoverImage).HasColumnType("LONGTEXT");
            });

            modelBuilder.Entity<QRCode>(e =>
            {
                e.HasKey(q => q.Id);
                e.Property(q => q.MainId).IsRequired().HasMaxLength(80);
                e.Property(q => q.CopyId).IsRequired().HasMaxLength(120);
                e.Property(q => q.QRCodeId).IsRequired().HasColumnType("LONGTEXT");
                e.HasIndex(q => q.CopyId).IsUnique();
                e.HasOne(q => q.Book)
                    .WithMany(b => b.QRCodes)
                    .HasForeignKey(q => q.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BorrowTransaction>(e =>
            {
                e.HasKey(t => t.Id);
                e.Property(t => t.Status).HasConversion<string>();
                e.Property(t => t.FineAmount).HasColumnType("decimal(18,2)");
                e.Property(t => t.Notes).HasMaxLength(500);
                e.HasOne(t => t.Book)
                    .WithMany(b => b.BorrowTransactions)
                    .HasForeignKey(t => t.BookId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(t => t.User)
                    .WithMany(u => u.BorrowTransactions)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(t => t.QRCode)
                    .WithMany(q => q.BorrowTransactions)
                    .HasForeignKey(t => t.QRCodeRecordId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(t => t.VerifiedByUser)
                    .WithMany(u => u.VerifiedForBorrowTransactions)
                    .HasForeignKey(t => t.VerifiedByUserId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Reservation>(e =>
            {
                e.HasKey(r => r.Id);
                e.Property(r => r.Status).HasConversion<string>();
                e.Property(r => r.Quantity).HasDefaultValue(1);
                e.Property(r => r.Notes).HasMaxLength(500);
                e.HasOne(r => r.Book)
                    .WithMany(b => b.Reservations)
                    .HasForeignKey(r => r.BookId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(r => r.User)
                    .WithMany(u => u.Reservations)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(r => r.QRCode)
                    .WithMany(q => q.Reservations)
                    .HasForeignKey(r => r.QRCodeRecordId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
