using HomeTrack.Domain.Account;
namespace HomeTrack.Application.Interface
{
    public interface ITokenRepository
    {
        Task AddAsync(ConfirmationToken token);
        Task<ConfirmationToken?> GetByTokenAsync(string token);
        Task MarkAsUsedAsync(string token);
        Task<int> SaveChangesAsync();
    }
}