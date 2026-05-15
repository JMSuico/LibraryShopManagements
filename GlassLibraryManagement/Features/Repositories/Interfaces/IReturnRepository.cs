using GlassLibraryManagement.Features.Data.Models;

namespace GlassLibraryManagement.Features.Repositories.Interfaces
{
    public interface IReturnRepository
    {
        /// <summary>Completed returns only.</summary>
        Task<IEnumerable<BorrowTransaction>> GetReturnedAsync(int? userIdFilter = null);
        Task<BorrowTransaction?> GetActiveByIdAsync(int transactionId);
        Task UpdateAsync(BorrowTransaction transaction);
    }
}