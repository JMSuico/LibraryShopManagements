using Microsoft.EntityFrameworkCore;
using GlassLibraryManagement.Features.Data;
using GlassLibraryManagement.Features.Data.Enums;
using GlassLibraryManagement.Features.Data.Models;
using GlassLibraryManagement.Features.Repositories.Interfaces;

namespace GlassLibraryManagement.Features.Repositories.Implementations
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync(int? userIdFilter = null)
        {
            IQueryable<Reservation> query = _context.Reservations
                .AsNoTracking()
                .Include(r => r.Book)
                .Include(r => r.User);

            if (userIdFilter.HasValue)
            {
                query = query.Where(r => r.UserId == userIdFilter.Value);
            }

            return await query
                .OrderByDescending(r => r.ReservedAt)
                .ToListAsync();
        }

        public async Task<Reservation?> GetByIdAsync(int id)
            => await _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<Reservation?> GetActiveByBookAndUserAsync(int bookId, int userId)
            => await _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r =>
                    r.BookId == bookId &&
                    r.UserId == userId &&
                    r.Status == ReservationStatus.Reserved &&
                    r.FulfilledAt == null &&
                    r.ReleasedAt == null);

        public async Task<int> SumActiveReservedQuantityByUserAsync(int userId)
            => await _context.Reservations
                .Where(r =>
                    r.UserId == userId &&
                    r.Status == ReservationStatus.Reserved &&
                    r.FulfilledAt == null &&
                    r.ReleasedAt == null)
                .SumAsync(r => r.Quantity);

        public async Task AddAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            var existing = await _context.Reservations.FindAsync(reservation.Id);
            if (existing is null) return;

            existing.FulfilledAt = reservation.FulfilledAt;
            existing.ReleasedAt = reservation.ReleasedAt;
            existing.Status = reservation.Status;
            existing.Quantity = reservation.Quantity;
            existing.Notes = reservation.Notes;
            await _context.SaveChangesAsync();
        }
    }
}
