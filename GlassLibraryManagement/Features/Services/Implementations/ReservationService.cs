using GlassLibraryManagement.Features.Data.Enums;
using GlassLibraryManagement.Features.Data.Models;
using GlassLibraryManagement.Features.Helpers;
using GlassLibraryManagement.Features.Repositories.Interfaces;
using GlassLibraryManagement.Features.Services.Interfaces;

namespace GlassLibraryManagement.Features.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IBorrowAndReturnRepository _borrowAndReturnRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;

        public ReservationService(
            IReservationRepository reservationRepository,
            IBorrowAndReturnRepository borrowAndReturnRepository,
            IBookRepository bookRepository,
            IUserRepository userRepository)
        {
            _reservationRepository = reservationRepository;
            _borrowAndReturnRepository = borrowAndReturnRepository;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
        }

        public Task<IEnumerable<Reservation>> GetReservationsAsync(int? forUserId = null)
            => _reservationRepository.GetAllAsync(forUserId);

        public Task<Reservation?> GetActiveReservationAsync(int bookId, int userId)
            => _reservationRepository.GetActiveByBookAndUserAsync(bookId, userId);

        public async Task ReserveAsync(int bookId, int userId, int quantity)
        {
            if (quantity < 1)
            {
                throw new InvalidOperationException("Reserve at least one copy.");
            }

            var book = await _bookRepository.GetByIdAsync(bookId)
                ?? throw new InvalidOperationException("Book not found.");
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");

            var existing = await _reservationRepository.GetActiveByBookAndUserAsync(bookId, userId);
            var maxForThisBook = book.TotalCopies;
            if (quantity > maxForThisBook)
            {
                throw new InvalidOperationException($"You can reserve at most {maxForThisBook} copy/copies for this title.");
            }

            var totalReservedElsewhere = await _reservationRepository.SumActiveReservedQuantityByUserAsync(userId);
            var reservedOtherBooks = totalReservedElsewhere - (existing?.Quantity ?? 0);
            var activeBorrowCount = await _borrowAndReturnRepository.CountActiveByUserAsync(userId);
            if (activeBorrowCount + reservedOtherBooks + quantity > user.BorrowLimit)
            {
                throw new InvalidOperationException($"{user.FullName} would exceed the hold limit of {user.BorrowLimit} items (borrows + reservations).");
            }

            if (existing != null)
            {
                var delta = quantity - existing.Quantity;
                existing.Quantity = quantity;
                existing.Notes = $"Reserved {quantity} copy/copies (catalog).";
                await _reservationRepository.UpdateAsync(existing);

                book.ReservedCopies += delta;
                BookInventoryHelper.Normalize(book);
                await _bookRepository.UpdateAsync(book);
                return;
            }

            var reservation = new Reservation
            {
                BookId = book.Id,
                UserId = user.Id,
                QRCodeRecordId = null,
                Quantity = quantity,
                ReservedAt = DateHelper.UtcNow(),
                Status = ReservationStatus.Reserved,
                Notes = $"Reserved {quantity} copy/copies (catalog)."
            };

            await _reservationRepository.AddAsync(reservation);

            book.ReservedCopies += quantity;
            BookInventoryHelper.Normalize(book);
            await _bookRepository.UpdateAsync(book);
        }

        public async Task ReleaseAsync(int reservationId, int? quantityToRelease = null)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId)
                ?? throw new InvalidOperationException("Reservation not found.");

            if (reservation.Status != ReservationStatus.Reserved || reservation.ReleasedAt != null || reservation.FulfilledAt != null)
            {
                throw new InvalidOperationException("Only active reservations can be released.");
            }

            var book = reservation.Book ?? await _bookRepository.GetByIdAsync(reservation.BookId)
                ?? throw new InvalidOperationException("Reserved book not found.");

            var qty = Math.Max(1, reservation.Quantity);
            var releaseQty = quantityToRelease ?? qty;
            
            if (releaseQty > qty)
            {
                throw new InvalidOperationException("Cannot release more copies than reserved.");
            }

            if (releaseQty == qty)
            {
                reservation.ReleasedAt = DateHelper.UtcNow();
                reservation.Status = ReservationStatus.Released;
                reservation.Notes = "Released.";
            }
            else
            {
                reservation.Quantity -= releaseQty;
                reservation.Notes = $"Released {releaseQty} copies, {reservation.Quantity} remaining.";
            }
            
            await _reservationRepository.UpdateAsync(reservation);

            book.ReservedCopies = Math.Max(0, book.ReservedCopies - releaseQty);
            BookInventoryHelper.Normalize(book);
            await _bookRepository.UpdateAsync(book);
        }

        public async Task FulfillAsync(int reservationId, int borrowDays, int? verifiedByUserId = null)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId)
                ?? throw new InvalidOperationException("Reservation not found.");

            if (reservation.Status != ReservationStatus.Reserved || reservation.ReleasedAt != null || reservation.FulfilledAt != null)
            {
                throw new InvalidOperationException("Only active reservations can be fulfilled.");
            }

            var user = reservation.User ?? await _userRepository.GetByIdAsync(reservation.UserId)
                ?? throw new InvalidOperationException("Reservation user not found.");
            var book = reservation.Book ?? await _bookRepository.GetByIdAsync(reservation.BookId)
                ?? throw new InvalidOperationException("Reserved book not found.");

            var activeBorrowCount = await _borrowAndReturnRepository.CountActiveByUserAsync(user.Id);
            if (activeBorrowCount >= user.BorrowLimit)
            {
                throw new InvalidOperationException($"{user.FullName} reached the borrow limit of {user.BorrowLimit} books.");
            }

            var borrowedAt = DateHelper.UtcNow();
            var days = borrowDays > 0 ? borrowDays : DateHelper.DefaultBorrowDays;
            var transaction = new BorrowTransaction
            {
                BookId = book.Id,
                UserId = user.Id,
                VerifiedByUserId = verifiedByUserId,
                QRCodeRecordId = null,
                BorrowedAt = borrowedAt,
                DueAt = DateHelper.CalculateDueDate(borrowedAt, days),
                Status = TransactionStatus.Borrowed,
                FineAmount = 0m,
                Notes = $"Fulfilled reservation for {days} day(s)."
            };

            await _borrowAndReturnRepository.AddTransactionAsync(transaction);

            var remaining = Math.Max(1, reservation.Quantity) - 1;
            book.ReservedCopies = Math.Max(0, book.ReservedCopies - 1);
            book.BorrowedCopies += 1;
            BookInventoryHelper.Normalize(book);
            await _bookRepository.UpdateAsync(book);

            if (remaining <= 0)
            {
                reservation.FulfilledAt = borrowedAt;
                reservation.Status = ReservationStatus.Fulfilled;
                reservation.Notes = "Fulfilled.";
            }
            else
            {
                reservation.Quantity = remaining;
                reservation.Notes = $"{remaining} copy/copies still on hold.";
            }

            await _reservationRepository.UpdateAsync(reservation);
        }
    }
}
