using GlassLibraryManagement.Features.Data.Models;
using GlassLibraryManagement.Features.Data.Enums;
using GlassLibraryManagement.Features.Repositories.Interfaces;
using GlassLibraryManagement.Features.Services.Interfaces;

namespace GlassLibraryManagement.Features.Services.Implementations
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

        public async Task<User?> AuthenticateAsync(string username, string password, RoleType role)
            => await _userRepository.AuthenticateAsync(username, password, role);

        public async Task<User?> GetUserByIdAsync(int id)
            => await _userRepository.GetByIdAsync(id);

        public async Task<User?> GetUserByUsernameAsync(string username)
            => await _userRepository.GetByUsernameAsync(username);

        public async Task CreateUserAsync(User user)
            => await _userRepository.AddAsync(user);

        public async Task UpdateUserAsync(User user)
            => await _userRepository.UpdateAsync(user);

        public async Task DeleteUserAsync(int id)
            => await _userRepository.DeleteAsync(id);
    }
}
