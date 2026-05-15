using Microsoft.EntityFrameworkCore;
using GlassLibraryManagement.Features.Data;
using GlassLibraryManagement.Features.Data.Enums;
using GlassLibraryManagement.Features.Data.Models;
using GlassLibraryManagement.Features.Repositories.Interfaces;

namespace GlassLibraryManagement.Features.Repositories.Implementations
{
    public class ReturnRepository : IReturnRepository
    {
        private readonly AppDbContext _context;

        public ReturnRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BorrowTransaction>> GetReturnedAsync(int? userIdFilter = null)
        {
            var query = _context.BorrowTransactions
                .AsNoTracking()
                .Include(t => t.Book)
                .Include(t => t.User)
                .Where(t => t.ReturnedAt != null);

            if (userIdFilter.HasValue)
            {
                query = query.Where(t => t.UserId == userIdFilter.Value);
            }

            return await query
                .OrderByDescending(t => t.ReturnedAt)
                .ToListAsync();
        }

        public async Task<BorrowTransaction?> GetActiveByIdAsync(int transactionId)
            => await _context.BorrowTransactions
                .Include(t => t.Book)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.Id == transactionId &&
                    (t.Status == TransactionStatus.Borrowed || t.Status == TransactionStatus.Overdue) &&
                    t.ReturnedAt == null);

        public async Task UpdateAsync(BorrowTransaction transaction)
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