using GlassLibraryManagement.Features.Data.Enums;

namespace GlassLibraryManagement.Features.Data.Models
{
    public class BorrowTransaction
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public int? VerifiedByUserId { get; set; }
        public int? QRCodeRecordId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime DueAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public TransactionStatus Status { get; set; } = TransactionStatus.Borrowed;
        public decimal FineAmount { get; set; }
        public string? Notes { get; set; }

        public Book? Book { get; set; }
        public User? User { get; set; }
        public User? VerifiedByUser { get; set; }
        public QRCode? QRCode { get; set; }
    }
}