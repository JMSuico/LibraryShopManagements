using GlassLibraryManagement.Features.Data.Enums;
using GlassLibraryManagement.Features.Data.Models;
using GlassLibraryManagement.Features.Helpers;
using GlassLibraryManagement.Features.Repositories.Interfaces;
using GlassLibraryManagement.Features.Services.Interfaces;

namespace GlassLibraryManagement.Features.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
            => await _bookRepository.GetAllAsync();

        public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
            => await _bookRepository.GetAvailableAsync();

        public async Task<Book?> GetBookByIdAsync(int id)
            => await _bookRepository.GetByIdAsync(id);

        public async Task CreateBookAsync(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.MainId))
            {
                book.MainId = QRHelper.GenerateMainId();
            }

            book.TotalCopies = Math.Max(0, book.TotalCopies);
            book.BorrowedCopies = 0;
            book.ReservedCopies = 0;

            for (int i = 1; i <= book.TotalCopies; i++)
            {
                var copyId = QRHelper.GenerateCopyId(book.MainId, i);
                book.QRCodes.Add(new QRCode
                {
                    MainId = book.MainId,
                    CopyId = copyId,
                    QRCodeId = QRHelper.GenerateQRCodeId(copyId),
                    IsAvailable = true
                });
            }

            BookInventoryHelper.Normalize(book);
            await _bookRepository.AddAsync(book);
        }

        public async Task UpdateBookAsync(Book book)
        {
            var existing = await _bookRepository.GetByIdAsync(book.Id)
                ?? throw new InvalidOperationException("Book not found.");

            if (string.IsNullOrWhiteSpace(book.MainId))
            {
                book.MainId = existing.MainId;
            }

            existing.MainId = book.MainId;
            existing.Title = book.Title;
            existing.Author = book.Author;
            existing.CoverImage = book.CoverImage;
            existing.TotalCopies = Math.Max(0, book.TotalCopies);

            if (existing.TotalCopies < existing.BorrowedCopies + existing.ReservedCopies)
            {
                throw new InvalidOperationException("Total copies cannot be less than the borrowed and reserved copies.");
            }

            // Adjust QRCodes based on TotalCopies
            int currentCopyCount = existing.QRCodes.Count;
            if (existing.TotalCopies > currentCopyCount)
            {
                // Find the highest copy number
                int maxCopyNum = 0;
                foreach (var code in existing.QRCodes)
                {
                    var parts = code.CopyId.Split("-COPY-");
                    if (parts.Length == 2 && int.TryParse(parts[1], out int num))
                    {
                        maxCopyNum = Math.Max(maxCopyNum, num);
                    }
                }

                int copiesToAdd = existing.TotalCopies - currentCopyCount;
                for (int i = 1; i <= copiesToAdd; i++)
                {
                    var copyId = QRHelper.GenerateCopyId(existing.MainId, maxCopyNum + i);
                    existing.QRCodes.Add(new QRCode
                    {
                        MainId = existing.MainId,
                        CopyId = copyId,
                        QRCodeId = QRHelper.GenerateQRCodeId(copyId),
                        IsAvailable = true
                    });
                }
            }
            else if (existing.TotalCopies < currentCopyCount)
            {
                int copiesToRemove = currentCopyCount - existing.TotalCopies;
                // Remove available copies first
                var copiesToRemoveList = existing.QRCodes
                    .Where(q => q.IsAvailable)
                    .OrderByDescending(q => q.CopyId)
                    .Take(copiesToRemove)
                    .ToList();

                if (copiesToRemoveList.Count < copiesToRemove)
                {
                    throw new InvalidOperationException("Cannot reduce Total Copies because some copies are currently borrowed or reserved.");
                }

                foreach (var copy in copiesToRemoveList)
                {
                    existing.QRCodes.Remove(copy);
                }
            }

            BookInventoryHelper.Normalize(existing);
            await _bookRepository.UpdateAsync(existing);
        }

        public async Task DeleteBookAsync(int id)
            => await _bookRepository.DeleteAsync(id);
    }
}
