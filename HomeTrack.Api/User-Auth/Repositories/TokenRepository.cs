
using HomeTrack.Domain.Account;
using HomeTrack.Application.Interface;
using HomeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeTrack.Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ApplicationDBContext _context;

        public TokenRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ConfirmationToken token)
        {
            await _context.ConfirmationTokens.AddAsync(token);
        }

        public async Task<ConfirmationToken?> GetByUserIdAndTokenAsync(int userId, string token)
        {
            return await _context.ConfirmationTokens
                                 .FirstOrDefaultAsync(t => t.UserId == userId && t.Token==token);
        }

        public async Task MarkAsUsedAsync(int tokenId)
        {
            var tokenEntity = await _context.ConfirmationTokens.
                FirstOrDefaultAsync(t => t.Id == tokenId);
            if (tokenEntity != null)
            {
                tokenEntity.Used = true;
            }
        }

        public async Task InvalidateActiveOtpTokensForUserAsync(int userId)
{
            var activeTokens = await _context.ConfirmationTokens
                .Where(t => t.UserId == userId && !t.Used)
                .ToListAsync();

            if (activeTokens.Any())
            {
                foreach (var token in activeTokens)
                {
                    token.Used = true; // Hoặc một trạng thái "Vô hiệu hóa" khác
                    // Hoặc token.ExpirationAt = DateTime.UtcNow; // Đặt hết hạn ngay lập tức
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