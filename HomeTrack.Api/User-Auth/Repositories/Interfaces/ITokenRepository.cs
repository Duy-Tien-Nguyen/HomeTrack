using HomeTrack.Domain.Account;
namespace HomeTrack.Application.Interface
{
    public interface ITokenRepository
    {
        Task AddAsync(ConfirmationToken token);
        Task<ConfirmationToken?> GetByUserIdAndTokenAsync(int userId, string token);
        Task MarkAsUsedAsync(int tokenId);
        Task InvalidateActiveOtpTokensForUserAsync(int userId);
        Task<int> SaveChangesAsync();
    }
}