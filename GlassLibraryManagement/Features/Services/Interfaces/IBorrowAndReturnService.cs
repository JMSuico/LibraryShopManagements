using GlassLibraryManagement.Features.Data.Models;

namespace GlassLibraryManagement.Features.Services.Interfaces
{
    public interface IBorrowAndReturnService
    {
        Task<IEnumerable<BorrowTransaction>> GetTransactionsAsync();
        Task BorrowAsync(int bookId, int userId, List<int> qrCodeIds, int borrowDays, int? verifiedByUserId = null);
        Task DirectBorrowAsync(int bookId, int userId, List<int> qrCodeIds, int borrowDays, int verifiedByUserId);
        Task ReturnAsync(int transactionId);
    }
}
