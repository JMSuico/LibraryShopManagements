using GlassLibraryManagement.Features.Data.Models;
using GlassLibraryManagement.Features.Data.Enums;

namespace GlassLibraryManagement.Features.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> AuthenticateAsync(string username, string password, RoleType role);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
    }
}
