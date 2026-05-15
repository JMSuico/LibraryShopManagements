using Microsoft.EntityFrameworkCore;
using GlassLibraryManagement.Features.Data;
using GlassLibraryManagement.Features.Data.Enums;
using GlassLibraryManagement.Features.Data.Models;
using GlassLibraryManagement.Features.Repositories.Interfaces;

namespace GlassLibraryManagement.Features.Repositories.Implementations
{
    public class BorrowRepository : IBorrowRepository
    {
        private readonly AppDbContext _context;

        public BorrowRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BorrowTransaction>> GetAllAsync()
            => await _context.BorrowTransactions
                .AsNoTracking()
                .Include(t => t.Book)
                .Include(t => t.User)
                .Include(t => t.VerifiedByUser)
                .OrderByDescending(t => t.BorrowedAt)
                .ToListAsync();

        public async Task<IEnumerable<BorrowTransaction>> GetByUserIdAsync(int userId)
            => await _context.BorrowTransactions
                .AsNoTracking()
                .Include(t => t.Book)
                .Include(t => t.User)
                .Include(t => t.VerifiedByUser)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.BorrowedAt)
                .ToListAsync();

        public async Task<IEnumerable<BorrowTransaction>> GetOutstandingAsync(int? userIdFilter = null)
        {
            var query = _context.BorrowTransactions
                .AsNoTracking()
                .Include(t => t.Book)
                .Include(t => t.User)
                .Include(t => t.VerifiedByUser)
                .Where(t => t.ReturnedAt == null);

            if (userIdFilter.HasValue)
            {
                query = query.Where(t => t.UserId == userIdFilter.Value);
            }

            return await query
                .OrderBy(t => t.DueAt)
                .ToListAsync();
        }

        public async Task<int> CountActiveByUserAsync(int userId)
            => await _context.BorrowTransactions.CountAsync(t =>
                t.UserId == userId &&
                (t.Status == TransactionStatus.Borrowed || t.Status == TransactionStatus.Overdue) &&
                t.ReturnedAt == null);

        public async Task AddAsync(BorrowTransaction transaction)
        {
            await _context.BorrowTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }
    }
}