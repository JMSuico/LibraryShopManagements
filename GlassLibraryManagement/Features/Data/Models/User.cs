using GlassLibraryManagement.Features.Data.Enums;

namespace GlassLibraryManagement.Features.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public RoleType Role { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<BorrowTransaction> BorrowTransactions { get; set; } = [];
        public ICollection<BorrowTransaction> VerifiedForBorrowTransactions { get; set; } = [];
        public ICollection<Reservation> Reservations { get; set; } = [];

        public string FullName => $"{FirstName} {LastName}".Trim();
        public int BorrowLimit => Role switch
        {
            RoleType.Admin => 10,
            RoleType.Librarian => 5,
            _ => 3
        };

        public bool MatchesLogin(string username, string password, RoleType role)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            return Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase)
                && Password == password
                && Role == role;
        }
    }
}
