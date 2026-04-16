namespace LibraryShopManagement.Features.Data.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? CoverImage { get; set; } // base64 data URL
    }
}
