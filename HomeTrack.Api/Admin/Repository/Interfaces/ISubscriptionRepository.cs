using HomeTrack.Domain; 

namespace HomeTrack.Application.Interface
{
  public interface ISubscriptionRepository
  {
    Task<Subscription?> GetByIdAsync(int id);
    Task<List<Subscription>> GetAllAsync();
    Task<List<Subscription>> GetByUserIdAsync(int userId);
    Task<Subscription?> GetActiveSubscriptionByUserIdAsync(int userId, int packageId); // Để kiểm tra user đã có gói active chưa
    Task<Subscription> AddAsync(Subscription subscription);
    Task UpdateAsync(Subscription subscription);
    Task<bool> DeleteAllByUser(int userId);
    Task<bool> ExistsAsync(int id);
  }
}