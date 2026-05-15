using GlassLibraryManagement.Features.Data.Models;
using GlassLibraryManagement.Features.Data.Enums;

namespace GlassLibraryManagement.Features.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User?> AuthenticateAsync(string username, string password, RoleType role);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
    }
}
