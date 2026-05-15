using GlassLibraryManagement.Features.Data.Models;

namespace GlassLibraryManagement.Features.Repositories.Interfaces
{
    public interface IBorrowRepository
    {
        Task<IEnumerable<BorrowTransaction>> GetAllAsync();
        Task<IEnumerable<BorrowTransaction>> GetByUserIdAsync(int userId);
        /// <summary>Active loans (not yet returned), optionally scoped to one user.</summary>
        Task<IEnumerable<BorrowTransaction>> GetOutstandingAsync(int? userIdFilter = null);
        Task<int> CountActiveByUserAsync(int userId);
        Task AddAsync(BorrowTransaction transaction);
    }
}