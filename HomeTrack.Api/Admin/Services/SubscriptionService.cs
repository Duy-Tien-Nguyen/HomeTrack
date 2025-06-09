using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using HomeTrack.Api.Request;

namespace HomeTrack.Application.Services
{
  public class SubscriptionService : ISubscriptionService
  {
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPackageRepository _packageRepository;
    private readonly IUserRepository _userReposotory;
    private readonly ILogger<SubscriptionService> _logger;
    public SubscriptionService(
      ISubscriptionRepository subscriptionRepository,
      IPackageRepository packageRepository,
      IUserRepository userReposotory,
      ILogger<SubscriptionService> logger)
    {
      _subscriptionRepository = subscriptionRepository;
      _packageRepository = packageRepository;
      _userReposotory = userReposotory;
      _logger = logger;
    }

    public async Task<Subscription?> GetByIdAsync(int id)
    {
      try
      {
        return await _subscriptionRepository.GetByIdAsync(id);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while getting subscription by ID: {SubscriptionId}", id);
        return null;
      }
    }
    public async Task<List<Subscription>> GetAllAsync()
    {
      try
      {
        return await _subscriptionRepository.GetAllAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while getting all subscriptions");
        return new List<Subscription>();
      }
    }
    public async Task<List<Subscription>> GetByUserIdAsync(int userId)
    {
      try
      {
        return await _subscriptionRepository.GetByUserIdAsync(userId);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while getting subscriptions by User ID: {UserId}", userId);
        return new List<Subscription>();
      }
    }
    public async Task<Subscription?> GetActiveSubscriptionByUserIdAsync(int userId, int packageId)
    {
      try
      {
        return await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId, packageId);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while getting active subscription by User ID: {UserId} and Package ID: {PackageId}", userId, packageId);
        return null;
      }
    }
    public async Task<SubscriptionDto> AddAsync(CreateSubscriptionDto subscriptionDto)
    {
      try
      {
        var package = await _packageRepository.GetByIdAsync(subscriptionDto.PackageId);
        if (package == null)
        {
          _logger.LogError("Package not found with ID: {PackageId}", subscriptionDto.PackageId);
          throw new Exception("Package not found");
        }

        var user = await _userReposotory.GetByIdAsync(subscriptionDto.UserId);
        if (user == null)
        {
          _logger.LogError("User not found with ID: {UserId}", subscriptionDto.UserId);
          throw new Exception("User not found");
        }

        if (await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(subscriptionDto.UserId, subscriptionDto.PackageId) != null)
        {
          _logger.LogWarning("Nguoi dùng với ID {UserId} đã có gói đăng ký hoạt động cho gói {PackageId}", subscriptionDto.UserId, subscriptionDto.PackageId);
          throw new Exception("Ban đã có gói đăng ký hoạt động cho gói này");
        }

        await _subscriptionRepository.DeleteAllByUser(user.Id);

        var subscription = new Subscription
        {
          UserId = subscriptionDto.UserId,
          PackageId = subscriptionDto.PackageId,
          StartsAt = DateTime.UtcNow,
          EndsAt = DateTime.UtcNow.AddDays(package.DurationDays),
          Status = SubscriptionStatus.Active
        };

        var result = await _subscriptionRepository.AddAsync(subscription);
        if (result == null)
        {
          _logger.LogError("Failed to add subscription for User ID: {UserId} and Package ID: {PackageId}", subscriptionDto.UserId, subscriptionDto.PackageId);
          throw new Exception("Tham gia đăng ký không thành công");
        }
        return new SubscriptionDto
        {
          Id = result.Id,
          UserId = result.UserId,
          UserName = user.FirstName + " " + user.LastName,
          PackageId = result.PackageId,
          PackageName = package.Name,
          PackagePrice = package.Price,
          Status = result.Status,
          StartsAt = result.StartsAt,
          EndsAt = result.EndsAt
        };
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while adding a new subscription");
        throw new InvalidOperationException("Lỗi xảy ra khi thêm đăng ký mới", ex);
      }
    }

    public async Task<SubscriptionDto> CancelAsync(int id, int userId)
    {
      try
      {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        if (subscription == null || subscription.UserId != userId)
        {
          _logger.LogWarning("Subscription not found or does not belong to user with ID: {UserId}", userId);
          throw new InvalidOperationException("Subscription not found or does not belong to the user");
        }

        subscription.Status = SubscriptionStatus.Cancelled;
        subscription.UpdatedAt = DateTime.UtcNow;

        await _subscriptionRepository.UpdateAsync(subscription);

        return new SubscriptionDto
        {
          Id = subscription.Id,
          UserId = subscription.UserId,
          PackageId = subscription.PackageId,
          Status = subscription.Status,
          StartsAt = subscription.StartsAt,
          EndsAt = subscription.EndsAt
        };
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while cancelling subscription with ID: {SubscriptionId}", id);
        throw;
      }
    }

    public async Task<SubscriptionDto> ExpireAsync(int id, int userId)
    {
      try
      {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        if (subscription == null || subscription.UserId != userId)
        {
          _logger.LogWarning("Subscription not found or does not belong to user with ID: {UserId}", userId);
          throw new Exception("Subscription not found or does not belong to the user");
        }

        subscription.Status = SubscriptionStatus.Expired;
        subscription.UpdatedAt = DateTime.UtcNow;

        await _subscriptionRepository.UpdateAsync(subscription);

        return new SubscriptionDto
        {
          Id = subscription.Id,
          UserId = subscription.UserId,
          PackageId = subscription.PackageId,
          Status = subscription.Status,
          StartsAt = subscription.StartsAt,
          EndsAt = subscription.EndsAt
        };
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while expiring subscription with ID: {SubscriptionId}", id);
        throw;
      }
    }

    public async Task<bool> ActivateAsync(int id, int userId)
    {
      try
      {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        if (subscription == null || subscription.UserId != userId)
        {
          _logger.LogWarning("Subscription not found or does not belong to user with ID: {UserId}", userId);
          throw new Exception("Goi đăng ký không tồn tại hoặc không thuộc về người dùng này");
        }

        subscription.Status = SubscriptionStatus.Active;
        subscription.UpdatedAt = DateTime.UtcNow;

        await _subscriptionRepository.UpdateAsync(subscription);
        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while activating subscription with ID: {SubscriptionId}", id);
        return false;
      }
    }
  }
}