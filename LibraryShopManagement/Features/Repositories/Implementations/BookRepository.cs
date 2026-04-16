using Microsoft.EntityFrameworkCore;
using LibraryShopManagement.Features.Data;
using LibraryShopManagement.Features.Data.Models;
using LibraryShopManagement.Features.Repositories.Interfaces;

namespace LibraryShopManagement.Features.Repositories.Implementations
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        public BookRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Book>> GetAllAsync()
            => await _context.Books.AsNoTracking().ToListAsync();

        public async Task<Book?> GetByIdAsync(int id)
            => await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            var existing = await _context.Books.FindAsync(book.Id);
            if (existing is null) return;
            existing.Title = book.Title;
            existing.Author = book.Author;
            existing.Price = book.Price;
            existing.Stock = book.Stock;
            existing.CoverImage = book.CoverImage;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null) return;
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}
