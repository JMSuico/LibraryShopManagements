using GlassLibraryManagement.Features.Data.Enums;

namespace GlassLibraryManagement.Features.Data.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public int? QRCodeRecordId { get; set; }
        public DateTime ReservedAt { get; set; }
        public DateTime? FulfilledAt { get; set; }
        public DateTime? ReleasedAt { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Reserved;
        /// <summary>Number of copies on hold for this reservation row.</summary>
        public int Quantity { get; set; } = 1;
        public string? Notes { get; set; }

        public Book? Book { get; set; }
        public User? User { get; set; }
        public QRCode? QRCode { get; set; }
    }
}
