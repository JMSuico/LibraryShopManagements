using System.ComponentModel.DataAnnotations.Schema;
using GlassLibraryManagement.Features.Data.Enums;

namespace GlassLibraryManagement.Features.Data.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string MainId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public int BorrowedCopies { get; set; }
        public int ReservedCopies { get; set; }
        public BookStatus Status { get; set; } = BookStatus.Available;
        public string? CoverImage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public int Quantity
        {
            get => AvailableCopies;
            set => AvailableCopies = value;
        }

        public ICollection<QRCode> QRCodes { get; set; } = [];
        public ICollection<BorrowTransaction> BorrowTransactions { get; set; } = [];
        public ICollection<Reservation> Reservations { get; set; } = [];
    }
}
