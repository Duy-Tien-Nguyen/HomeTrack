// using HomeTrack.Domain; // <-- Xóa hoặc comment
using HomeTrack.Api.Models.Entities; // <-- Đảm bảo dòng này

namespace HomeTrack.Application.Interface
{
  public interface ISubscriptionRepository
  {
    Task<Subscription?> GetByIdAsync(int id);
    Task<IEnumerable<Subscription>> GetAllAsync();
    Task<IEnumerable<Subscription>> GetByUserIdAsync(int userId);
    Task<Subscription?> GetActiveSubscriptionByUserIdAsync(int userId, int packageId);
    Task<Subscription> AddAsync(Subscription subscription);
    Task UpdateAsync(Subscription subscription);
    Task<bool> ExistsAsync(int id);
  }
}