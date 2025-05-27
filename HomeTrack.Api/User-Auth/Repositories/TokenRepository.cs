
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

        public async Task<ConfirmationToken?> GetByTokenAsync(string tokenValue)
        {
            return await _context.ConfirmationTokens
                                 .FirstOrDefaultAsync(t => t.Token == tokenValue);
        }

        public async Task MarkAsUsedAsync(string tokenValue)
        {
            var tokenEntity = await _context.ConfirmationTokens.
                FirstOrDefaultAsync(t => t.Token == tokenValue);
            if (tokenEntity != null)
            {
                tokenEntity.Used = true;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}