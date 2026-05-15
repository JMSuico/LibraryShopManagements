using Microsoft.EntityFrameworkCore;
using GlassLibraryManagement.Features.Data;
using GlassLibraryManagement.Features.Data.Enums;
using GlassLibraryManagement.Features.Data.Models;
using GlassLibraryManagement.Features.Repositories.Interfaces;

namespace GlassLibraryManagement.Features.Repositories.Implementations
{
    public class BorrowAndReturnRepository : IBorrowAndReturnRepository
    {
        private readonly AppDbContext _context;

        public BorrowAndReturnRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BorrowTransaction>> GetAllTransactionsAsync()
            => await _context.BorrowTransactions
                .AsNoTracking()
                .Include(t => t.Book)
                .Include(t => t.User)
                .Include(t => t.VerifiedByUser)
                .OrderByDescending(t => t.BorrowedAt)
                .ToListAsync();

        public async Task<int> CountActiveByUserAsync(int userId)
            => await _context.BorrowTransactions.CountAsync(t =>
                t.UserId == userId &&
                (t.Status == TransactionStatus.Borrowed || t.Status == TransactionStatus.Overdue) &&
                t.ReturnedAt == null);

        public async Task AddTransactionAsync(BorrowTransaction transaction)
        {
            await _context.BorrowTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransactionAsync(BorrowTransaction transaction)
        {
            var existing = await _context.BorrowTransactions.FindAsync(transaction.Id);
            if (existing is null) return;

            existing.ReturnedAt = transaction.ReturnedAt;
            existing.Status = transaction.Status;
            existing.FineAmount = transaction.FineAmount;
            existing.Notes = transaction.Notes;
            await _context.SaveChangesAsync();
        }
    }
}
