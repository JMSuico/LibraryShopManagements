using LibraryShopManagement.Features.Data.Models;
using LibraryShopManagement.Features.Repositories.Interfaces;
using LibraryShopManagement.Features.Services.Interfaces;

namespace LibraryShopManagement.Features.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
            => await _userRepository.GetAllAsync();

        public async Task<User?> GetUserByIdAsync(int id)
            => await _userRepository.GetByIdAsync(id);

        public async Task CreateUserAsync(User user)
            => await _userRepository.AddAsync(user);

        public async Task UpdateUserAsync(User user)
            => await _userRepository.UpdateAsync(user);

        public async Task DeleteUserAsync(int id)
            => await _userRepository.DeleteAsync(id);
    }
}
