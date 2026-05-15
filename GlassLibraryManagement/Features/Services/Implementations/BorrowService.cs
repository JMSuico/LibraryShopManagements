using GlassLibraryManagement.Features.Data.Enums;
using GlassLibraryManagement.Features.Data.Models;
using GlassLibraryManagement.Features.Helpers;
using GlassLibraryManagement.Features.Repositories.Interfaces;
using GlassLibraryManagement.Features.Services.Interfaces;

namespace GlassLibraryManagement.Features.Services.Implementations
{
    public class BorrowService : IBorrowService
    {
        private readonly IBorrowRepository _borrowRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;

        public BorrowService(
            IBorrowRepository borrowRepository,
            IBookRepository bookRepository,
            IUserRepository userRepository)
        {
            _borrowRepository = borrowRepository;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<BorrowTransaction>> GetBorrowLogsAsync()
        {
            var transactions = (await _borrowRepository.GetAllAsync()).ToList();
            ApplyOverdueState(transactions);
            return transactions;
        }

        public async Task<IEnumerable<BorrowTransaction>> GetMyBorrowingsAsync(int userId)
        {
            var transactions = (await _borrowRepository.GetByUserIdAsync(userId)).ToList();
            ApplyOverdueState(transactions);
            return transactions;
        }

        public async Task<IEnumerable<BorrowTransaction>> GetTransactionsAsync()
        {
            var transactions = (await _borrowRepository.GetAllAsync()).ToList();
            ApplyOverdueState(transactions);
            return transactions;
        }

        public async Task BorrowAsync(int bookId, int userId, List<int> qrCodeIds, int borrowDays = DateHelper.DefaultBorrowDays, int? verifiedByUserId = null)
        {
            int quantity = qrCodeIds.Count;
            if (quantity < 1)
            {
                throw new InvalidOperationException("Borrow at least one copy.");
            }

            var book = await _bookRepository.GetByIdAsync(bookId)
                ?? throw new InvalidOperationException("Book not found.");
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("Borrower not found.");

            if (book.AvailableCopies < quantity)
            {
                throw new InvalidOperationException($"Not enough available copies. Requested {quantity}, available {book.AvailableCopies}.");
            }

            var activeBorrowCount = await _borrowRepository.CountActiveByUserAsync(userId);
            if (activeBorrowCount + quantity > user.BorrowLimit)
            {
                throw new InvalidOperationException($"{user.FullName} reached the borrow limit. Current: {activeBorrowCount}, Requested: {quantity}, Limit: {user.BorrowLimit}.");
            }

            var borrowedAt = DateHelper.UtcNow();
            var days = borrowDays > 0 ? borrowDays : DateHelper.DefaultBorrowDays;
            
            foreach (var qrCodeId in qrCodeIds)
            {
                var qrCode = book.QRCodes.FirstOrDefault(q => q.Id == qrCodeId);
                if (qrCode == null || !qrCode.IsAvailable)
                {
                    throw new InvalidOperationException("One or more selected copies are not available.");
                }
                
                qrCode.IsAvailable = false;

                var transaction = new BorrowTransaction
                {
                    BookId = book.Id,
                    UserId = user.Id,
                    VerifiedByUserId = verifiedByUserId,
                    QRCodeRecordId = qrCodeId,
                    BorrowedAt = borrowedAt,
                    DueAt = DateHelper.CalculateDueDate(borrowedAt, days),
                    Status = TransactionStatus.Borrowed,
                    FineAmount = 0m,
                    Notes = $"Borrowed for {days} day(s)."
                };
                await _borrowRepository.AddAsync(transaction);
            }

            book.BorrowedCopies += quantity;
            BookInventoryHelper.Normalize(book);
            await _bookRepository.UpdateAsync(book);
        }

        private static void ApplyOverdueState(List<BorrowTransaction> transactions)
        {
            var now = DateHelper.UtcNow();
            foreach (var transaction in transactions.Where(t => t.ReturnedAt == null && t.DueAt < now))
            {
                transaction.Status = TransactionStatus.Overdue;
                transaction.FineAmount = FineHelper.CalculateFine(transaction.DueAt, now);
            }
        }
    }
}
