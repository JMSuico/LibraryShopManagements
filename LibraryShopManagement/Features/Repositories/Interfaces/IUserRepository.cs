using LibraryShopManagement.Features.Data.Models;

namespace LibraryShopManagement.Features.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<User?> GetByIdAsync(int id);
    }
}
