using GlassLibraryManagement.Features.Data.Enums;
using GlassLibraryManagement.Features.Data.Models;

namespace GlassLibraryManagement.Features.Helpers
{
    public static class BookInventoryHelper
    {
        public static void Normalize(Book book)
        {
            book.TotalCopies = Math.Max(0, book.TotalCopies);
            book.BorrowedCopies = Math.Max(0, book.BorrowedCopies);
            book.ReservedCopies = Math.Max(0, book.ReservedCopies);

            if (book.BorrowedCopies + book.ReservedCopies > book.TotalCopies)
            {
                throw new InvalidOperationException("Borrowed and reserved copies cannot exceed the total copies.");
            }

            book.AvailableCopies = Math.Max(0, book.TotalCopies - book.BorrowedCopies - book.ReservedCopies);
            book.Status = DetermineStatus(book);
        }

        public static BookStatus DetermineStatus(Book book)
        {
            if (book.AvailableCopies > 0)
            {
                return BookStatus.Available;
            }

            if (book.ReservedCopies > 0)
            {
                return BookStatus.Reserved;
            }

            if (book.BorrowedCopies > 0)
            {
                return BookStatus.Borrowed;
            }

            return BookStatus.Available;
        }
    }
}
