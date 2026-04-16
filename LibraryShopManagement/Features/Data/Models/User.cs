using LibraryShopManagement.Features.Data.Enums;

namespace LibraryShopManagement.Features.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public UserRole Role { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
