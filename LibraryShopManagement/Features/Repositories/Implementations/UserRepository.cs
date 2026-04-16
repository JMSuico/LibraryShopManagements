using Microsoft.EntityFrameworkCore;
using LibraryShopManagement.Features.Data;
using LibraryShopManagement.Features.Data.Models;
using LibraryShopManagement.Features.Repositories.Interfaces;

namespace LibraryShopManagement.Features.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) => _context = context;

        // AsNoTracking so EF never holds a reference — avoids the "already tracked" error
        public async Task<IEnumerable<User>> GetAllAsync()
            => await _context.Users.AsNoTracking().ToListAsync();

        public async Task<User?> GetByIdAsync(int id)
            => await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // Find tracked entity and copy values — never attach a detached object
        public async Task UpdateAsync(User user)
        {
            var existing = await _context.Users.FindAsync(user.Id);
            if (existing is null) return;
            existing.Role = user.Role;
            existing.FirstName = user.FirstName;
            existing.LastName = user.LastName;
            existing.Address = user.Address;
            existing.Username = user.Username;
            existing.Email = user.Email;
            existing.Password = user.Password;
            existing.ProfileImage = user.ProfileImage;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
