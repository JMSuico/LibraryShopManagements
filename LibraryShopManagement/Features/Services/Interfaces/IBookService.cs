using LibraryShopManagement.Features.Data.Models;

namespace LibraryShopManagement.Features.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetBooksAsync();
        Task CreateBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
        Task<Book?> GetBookByIdAsync(int id);
    }
}
