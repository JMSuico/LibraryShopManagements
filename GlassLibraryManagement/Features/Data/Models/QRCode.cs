namespace GlassLibraryManagement.Features.Data.Models
{
    public class QRCode
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string MainId { get; set; } = string.Empty;
        public string CopyId { get; set; } = string.Empty;
        public string QRCodeId { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;

        public Book? Book { get; set; }
        public ICollection<BorrowTransaction> BorrowTransactions { get; set; } = [];
        public ICollection<Reservation> Reservations { get; set; } = [];
    }
}
