using GlassLibraryManagement.Features.Data.Models;

namespace GlassLibraryManagement.Features.Repositories.Interfaces
{
    public interface IBorrowAndReturnRepository
    {
        Task<IEnumerable<BorrowTransaction>> GetAllTransactionsAsync();
        Task<int> CountActiveByUserAsync(int userId);
        Task AddTransactionAsync(BorrowTransaction transaction);
        Task UpdateTransactionAsync(BorrowTransaction transaction);
    }
}
