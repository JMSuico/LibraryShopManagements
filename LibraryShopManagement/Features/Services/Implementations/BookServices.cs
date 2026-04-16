using LibraryShopManagement.Features.Data.Models;
using LibraryShopManagement.Features.Repositories.Interfaces;
using LibraryShopManagement.Features.Services.Interfaces;

namespace LibraryShopManagement.Features.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
            => await _bookRepository.GetAllAsync();

        public async Task<Book?> GetBookByIdAsync(int id)
            => await _bookRepository.GetByIdAsync(id);

        public async Task CreateBookAsync(Book book)
            => await _bookRepository.AddAsync(book);

        public async Task UpdateBookAsync(Book book)
            => await _bookRepository.UpdateAsync(book);

        public async Task DeleteBookAsync(int id)
            => await _bookRepository.DeleteAsync(id);
    }
}
