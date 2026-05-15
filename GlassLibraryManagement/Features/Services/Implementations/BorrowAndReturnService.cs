using GlassLibraryManagement.Features.Data.Enums;
using GlassLibraryManagement.Features.Data.Models;
using GlassLibraryManagement.Features.Helpers;
using GlassLibraryManagement.Features.Repositories.Interfaces;
using GlassLibraryManagement.Features.Services.Interfaces;

namespace GlassLibraryManagement.Features.Services.Implementations
{
    public class BorrowAndReturnService : IBorrowAndReturnService
    {
        private readonly IBorrowAndReturnRepository _repository;
        private readonly IBorrowService _borrowService;
        private readonly IReturnService _returnService;

        public BorrowAndReturnService(
            IBorrowAndReturnRepository repository,
            IBorrowService borrowService,
            IReturnService returnService)
        {
            _repository = repository;
            _borrowService = borrowService;
            _returnService = returnService;
        }

        public async Task<IEnumerable<BorrowTransaction>> GetTransactionsAsync()
        {
            var transactions = (await _repository.GetAllTransactionsAsync()).ToList();
            var now = DateHelper.UtcNow();
            foreach (var transaction in transactions.Where(t => t.ReturnedAt == null && t.DueAt < now))
            {
                transaction.Status = TransactionStatus.Overdue;
                transaction.FineAmount = FineHelper.CalculateFine(transaction.DueAt, now);
            }

            return transactions;
        }

        public Task BorrowAsync(int bookId, int userId, List<int> qrCodeIds, int borrowDays, int? verifiedByUserId = null)
        {
            var days = borrowDays > 0 ? borrowDays : DateHelper.DefaultBorrowDays;
            return _borrowService.BorrowAsync(bookId, userId, qrCodeIds, days, verifiedByUserId);
        }

        public async Task DirectBorrowAsync(int bookId, int userId, List<int> qrCodeIds, int borrowDays, int verifiedByUserId)
        {
            if (qrCodeIds == null || qrCodeIds.Count < 1)
            {
                throw new InvalidOperationException("Borrow at least one copy.");
            }

            if (verifiedByUserId < 1)
            {
                throw new InvalidOperationException("Staff verifier account is required.");
            }

            var days = borrowDays > 0 ? borrowDays : DateHelper.DefaultBorrowDays;
            await _borrowService.BorrowAsync(bookId, userId, qrCodeIds, days, verifiedByUserId);
        }

        public Task ReturnAsync(int transactionId)
            => _returnService.ReturnAsync(transactionId);
    }
}
