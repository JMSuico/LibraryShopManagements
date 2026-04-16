using LibraryShopManagement.Features.Data.Models;

namespace LibraryShopManagement.Features.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);
        Task<Book?> GetByIdAsync(int id);
    }
}
