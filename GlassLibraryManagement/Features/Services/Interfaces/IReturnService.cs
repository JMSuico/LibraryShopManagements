using GlassLibraryManagement.Features.Data.Models;

namespace GlassLibraryManagement.Features.Services.Interfaces
{
    public interface IReturnService
    {
        /// <summary>Completed return transactions, optionally for one user.</summary>
        Task<IEnumerable<BorrowTransaction>> GetReturnLogsAsync(int? forUserId = null);
        Task<BorrowTransaction> GetReturnPreviewAsync(int transactionId);
        Task ReturnAsync(int transactionId);
    }
}
