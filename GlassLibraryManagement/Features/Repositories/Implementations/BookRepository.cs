using Microsoft.EntityFrameworkCore;
using GlassLibraryManagement.Features.Data;
using GlassLibraryManagement.Features.Data.Enums;
using GlassLibraryManagement.Features.Data.Models;
using GlassLibraryManagement.Features.Repositories.Interfaces;

namespace GlassLibraryManagement.Features.Repositories.Implementations
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
            => await _context.Books
                .Include(b => b.QRCodes)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Book>> GetAvailableAsync()
            => await _context.Books
                .Include(b => b.QRCodes)
                .AsNoTracking()
                .Where(b => b.AvailableCopies > 0 || b.Status == BookStatus.Available)
                .ToListAsync();

        public async Task<Book?> GetByIdAsync(int id)
            => await _context.Books
                .Include(b => b.QRCodes)
                .FirstOrDefaultAsync(b => b.Id == id);

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            var existing = await _context.Books.FindAsync(book.Id);
            if (existing is null) return;

            existing.MainId = book.MainId;
            existing.Title = book.Title;
            existing.Author = book.Author;
            existing.TotalCopies = book.TotalCopies;
            existing.AvailableCopies = book.AvailableCopies;
            existing.BorrowedCopies = book.BorrowedCopies;
            existing.ReservedCopies = book.ReservedCopies;
            existing.Status = book.Status;
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
