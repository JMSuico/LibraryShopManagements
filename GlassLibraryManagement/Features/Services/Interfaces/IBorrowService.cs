using GlassLibraryManagement.Features.Data.Models;
using GlassLibraryManagement.Features.Helpers;

namespace GlassLibraryManagement.Features.Services.Interfaces
{
    public interface IBorrowService
    {
        /// <summary>All borrow transactions (history) for staff borrow logs.</summary>
        Task<IEnumerable<BorrowTransaction>> GetBorrowLogsAsync();

        /// <summary>Current user’s loans and returns (overdue, on loan, returned).</summary>
        Task<IEnumerable<BorrowTransaction>> GetMyBorrowingsAsync(int userId);

        Task<IEnumerable<BorrowTransaction>> GetTransactionsAsync();
        Task BorrowAsync(int bookId, int userId, List<int> qrCodeIds, int borrowDays = DateHelper.DefaultBorrowDays, int? verifiedByUserId = null);
    }
}
