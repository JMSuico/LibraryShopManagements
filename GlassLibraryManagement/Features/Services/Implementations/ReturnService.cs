using GlassLibraryManagement.Features.Data.Enums;
using GlassLibraryManagement.Features.Data.Models;
using GlassLibraryManagement.Features.Helpers;
using GlassLibraryManagement.Features.Repositories.Interfaces;
using GlassLibraryManagement.Features.Services.Interfaces;

namespace GlassLibraryManagement.Features.Services.Implementations
{
    public class ReturnService : IReturnService
    {
        private readonly IReturnRepository _returnRepository;
        private readonly IBookRepository _bookRepository;

        public ReturnService(
            IReturnRepository returnRepository,
            IBookRepository bookRepository)
        {
            _returnRepository = returnRepository;
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<BorrowTransaction>> GetReturnLogsAsync(int? forUserId = null)
            => await _returnRepository.GetReturnedAsync(forUserId);

        public async Task<BorrowTransaction> GetReturnPreviewAsync(int transactionId)
        {
            var transaction = await _returnRepository.GetActiveByIdAsync(transactionId)
                ?? throw new InvalidOperationException("No active borrow transaction was found for this record.");

            return transaction;
        }

        public async Task ReturnAsync(int transactionId)
        {
            var transaction = await _returnRepository.GetActiveByIdAsync(transactionId)
                ?? throw new InvalidOperationException("No active borrow transaction was found for this record.");

            var returnedAt = DateHelper.UtcNow();
            transaction.ReturnedAt = returnedAt;
            transaction.Status = DateHelper.IsOverdue(transaction.DueAt, returnedAt)
                ? TransactionStatus.Overdue
                : TransactionStatus.Returned;
            transaction.FineAmount = FineHelper.CalculateFine(transaction.DueAt, returnedAt);
            transaction.Notes = "Returned.";
            
            if (transaction.QRCode != null)
            {
                transaction.QRCode.IsAvailable = true;
            }

            await _returnRepository.UpdateAsync(transaction);

            var book = transaction.Book ?? await _bookRepository.GetByIdAsync(transaction.BookId)
                ?? throw new InvalidOperationException("Book not found for the return transaction.");

            book.BorrowedCopies = Math.Max(0, book.BorrowedCopies - 1);
            BookInventoryHelper.Normalize(book);
            await _bookRepository.UpdateAsync(book);
        }
    }
}
