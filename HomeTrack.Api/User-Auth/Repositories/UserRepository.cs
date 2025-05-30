using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using Microsoft.EntityFrameworkCore;
using HomeTrack.Infrastructure.Data;
using System.Threading.Tasks;

namespace HomeTrack.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;

        public UserRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task ActivateUserAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                user.Status = Domain.Enum.UserStatus.Active;
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            // SaveChangesAsync sẽ được gọi ở Unit of Work hoặc Service layer
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}