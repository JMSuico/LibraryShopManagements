using GlassLibraryManagement.Features.Data.Models;

namespace GlassLibraryManagement.Features.Services.Interfaces
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetReservationsAsync(int? forUserId = null);
        Task<Reservation?> GetActiveReservationAsync(int bookId, int userId);
        Task ReserveAsync(int bookId, int userId, int quantity);
        Task ReleaseAsync(int reservationId, int? quantityToRelease = null);
        Task FulfillAsync(int reservationId, int borrowDays, int? verifiedByUserId = null);
    }
}
