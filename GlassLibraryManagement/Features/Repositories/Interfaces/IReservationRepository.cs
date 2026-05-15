using GlassLibraryManagement.Features.Data.Models;

namespace GlassLibraryManagement.Features.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync(int? userIdFilter = null);
        Task<Reservation?> GetByIdAsync(int id);
        Task<Reservation?> GetActiveByBookAndUserAsync(int bookId, int userId);
        /// <summary>Sum of <see cref="Reservation.Quantity"/> for active holds.</summary>
        Task<int> SumActiveReservedQuantityByUserAsync(int userId);
        Task AddAsync(Reservation reservation);
        Task UpdateAsync(Reservation reservation);
    }
}
