using GlassLibraryManagement.Features.Data.Models;

namespace GlassLibraryManagement.Features.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<IEnumerable<Book>> GetAvailableBooksAsync();
        Task CreateBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
        Task<Book?> GetBookByIdAsync(int id);
    }
}