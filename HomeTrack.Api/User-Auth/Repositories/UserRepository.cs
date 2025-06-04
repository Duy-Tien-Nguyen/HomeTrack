using HomeTrack.Application.Interface;
// using HomeTrack.Domain; // <-- Có thể xóa hoặc comment dòng này
using Microsoft.EntityFrameworkCore;
using HomeTrack.Infrastructure.Data;
using System.Threading.Tasks;
using HomeTrack.Api.Models.Entities; // <-- THÊM DÒNG NÀY

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
            // Đảm bảo FindAsync trả về Entity từ Models.Entities
            return await _context.Users.FindAsync(id);
        }

        public async Task ActivateUserAsync(int id)
        {
            // Đảm bảo tìm kiếm Entity từ Models.Entities
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                user.Status = Domain.Enum.UserStatus.Active; // Sử dụng UserStatus từ Domain.Enum
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            // Đảm bảo FirstOrDefaultAsync trả về Entity từ Models.Entities
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            // Đảm bảo AddAsync nhận vào Entity từ Models.Entities
            await _context.Users.AddAsync(user);
            // SaveChangesAsync sẽ được gọi ở Unit of Work hoặc Service layer
        }

        public void Update(User user)
        {
            // Đảm bảo Update nhận vào Entity từ Models.Entities
            _context.Users.Update(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}