using HomeTrack.Application.Interface;
// using HomeTrack.Domain.Account; // <-- Có thể xóa hoặc comment dòng này
using HomeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HomeTrack.Api.Models.Entities; // <-- THÊM DÒNG NÀY

namespace HomeTrack.Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ApplicationDBContext _context;

        public TokenRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(HomeTrack.Api.Models.Entities.ConfirmationToken token)
        {
            // Kiểm tra loại của token được truyền vào nếu cần
            await _context.ConfirmationTokens.AddAsync(token); // Đảm bảo đây là DbSet của Entity từ Models.Entities
        }

        public async Task<HomeTrack.Api.Models.Entities.ConfirmationToken?> GetByUserIdAndTokenAsync(int userId, string token)
        {
            // Đảm bảo FirstOrDefaultAsync trả về Entity từ Models.Entities
            return await _context.ConfirmationTokens
                                 .FirstOrDefaultAsync(t => t.UserId == userId && t.Token==token);
        }

        public async Task MarkAsUsedAsync(int tokenId)
        {
            // Đảm bảo tìm kiếm Entity từ Models.Entities
            var tokenEntity = await _context.ConfirmationTokens.
                FirstOrDefaultAsync(t => t.Id == tokenId);
            if (tokenEntity != null)
            {
                tokenEntity.Used = true; // Sử dụng thuộc tính Used của Entity từ Models.Entities
            }
        }

        public async Task InvalidateActiveOtpTokensForUserAsync(int userId)
        {
            // Đảm bảo tìm kiếm và làm việc với Entity từ Models.Entities
            var activeTokens = await _context.ConfirmationTokens
                .Where(t => t.UserId == userId && !t.Used)
                .ToListAsync();

            if (activeTokens.Any())
            {
                foreach (var token in activeTokens)
                {
                    token.Used = true; // Sử dụng thuộc tính Used của Entity từ Models.Entities
                }
                // Việc SaveChangesAsync sẽ được gọi bởi lớp Service sau khi tất cả các thao tác hoàn tất
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}