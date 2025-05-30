using HomeTrack.Domain;

namespace HomeTrack.Application.Interface
{
  public interface ISubscriptionRepository
  {
    Task<Subscription?> GetByIdAsync(int id);
    Task<IEnumerable<Subscription>> GetAllAsync();
    Task<IEnumerable<Subscription>> GetByUserIdAsync(int userId);
    Task<Subscription?> GetActiveSubscriptionByUserIdAsync(int userId, int packageId); // Để kiểm tra user đã có gói active chưa
    Task<Subscription> AddAsync(Subscription subscription);
    Task UpdateAsync(Subscription subscription);
    // Delete Subscription thường không phổ biến, có thể là cancel (update status)
    Task<bool> ExistsAsync(int id);
  }
}