using GlassLibraryManagement.Features.Data.Models;

namespace GlassLibraryManagement.Features.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<IEnumerable<Book>> GetAvailableAsync();
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);
        Task<Book?> GetByIdAsync(int id);
    }
}