using HomeTrack.Application.Interface;
using HomeTrack.Infrastructure.Data;
using HomeTrack.Domain;
using Microsoft.EntityFrameworkCore;

namespace HomeTrack.Infrastructure.Repositories
{
  public class SubscriptionRepository : ISubscriptionRepository
  {
    private readonly ApplicationDBContext _context;
    private readonly ILogger<SubscriptionRepository> _logger;
    public SubscriptionRepository(ApplicationDBContext context, ILogger<SubscriptionRepository> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task<Subscription?> GetByIdAsync(int id)
    {
      try
      {
        return await _context.Subscriptions.FindAsync(id);
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
        return await _context.Subscriptions.ToListAsync();
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
        return await _context.Subscriptions
          .Where(s => s.UserId == userId)
          .ToListAsync();
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
        return await _context.Subscriptions
          .Where(s => s.UserId == userId && s.PackageId == packageId && s.Status == SubscriptionStatus.Active)
          .FirstOrDefaultAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while getting active subscription by User ID: {UserId} and Package ID: {PackageId}", userId, packageId);
        return null;
      }
    }

    public async Task<Subscription> AddAsync(Subscription subscription)
    {
      try
      {
        subscription.CreatedAt = DateTime.UtcNow;
        subscription.UpdatedAt = DateTime.UtcNow;

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        return subscription;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while adding a new subscription");
        throw;
      }
    }

    public async Task UpdateAsync(Subscription subscription)
    {
      try
      {
        subscription.UpdatedAt = DateTime.UtcNow;
        _context.Subscriptions.Update(subscription);
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while updating subscription with ID: {SubscriptionId}", subscription.Id);
        throw;
      }
    }

    public async Task<bool> ExistsAsync(int id)
    {
      try
      {
        return await _context.Subscriptions.AnyAsync(s => s.Id == id);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while checking if subscription exists with ID: {SubscriptionId}", id);
        return false;
      }
    }

    public async Task<bool> DeleteAllByUser(int userId)
    {
      try
      {
        var subscriptions = await _context.Subscriptions
          .Where(s => s.UserId == userId)
          .ToListAsync();

        if (subscriptions.Count == 0)
        {
          return false; // No subscriptions found for the user
        }

        _context.Subscriptions.RemoveRange(subscriptions);
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while deleting all subscriptions for User ID: {UserId}", userId);
        return false;
      }
    }
  }
}