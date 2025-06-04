using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using HomeTrack.Api.Request;

namespace HomeTrack.Application.Interface
{
    public interface ISubscriptionService
    {
        Task<Subscription?> GetByIdAsync(int id);
        Task<List<Subscription>> GetAllAsync();
        Task<List<Subscription>> GetByUserIdAsync(int userId);
        Task<Subscription?> GetActiveSubscriptionByUserIdAsync(int userId, int packageId); // Để kiểm tra user đã có gói active chưa
        Task<SubscriptionDto> AddAsync(CreateSubscriptionDto subscriptionDto);
        Task<SubscriptionDto> CancelAsync(int id, int userId);  // Hủy gói đăng ký
        Task<SubscriptionDto> ExpireAsync(int id, int userId); // Gia hạn gói đăng ký
        Task<bool> ActivateAsync(int id, int userId); // Kích hoạt gói đăng ký
    }
}