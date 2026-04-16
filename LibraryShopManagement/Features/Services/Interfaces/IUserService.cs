using LibraryShopManagement.Features.Data.Models;

namespace LibraryShopManagement.Features.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<User?> GetUserByIdAsync(int id);
    }
}
