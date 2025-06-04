using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using HomeTrack.Api.Request;

namespace HomeTrack.Application.Services
{
  public class PackageService : IPackageService
  {
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPackageRepository _packageRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<PackageService> _logger;
    public PackageService(
      ISubscriptionRepository subscriptionRepository,
      IPackageRepository packageRepository,
      IUserRepository userRepository,
      ILogger<PackageService> logger)
    {
      _subscriptionRepository = subscriptionRepository;
      _packageRepository = packageRepository;
      _userRepository = userRepository;
      _logger = logger;
    }

    public async Task<Package?> GetByIdAsync(int id)
    {
      try
      {
        return await _packageRepository.GetByIdAsync(id);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while getting package by ID: {PackageId}", id);
        return null;
      }
    }

    public async Task<List<Package>?> GetAllAsync()
    {
      try
      {
        return await _packageRepository.GetAllAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while getting all packages");
        return new List<Package>();
      }
    }

    public async Task<Package> AddAsync(CreatePackageDto packageDto)
    {
      try
      {
        var package = new Package
        {
          Name = packageDto.Name,
          Description = packageDto.Description,
          Price = packageDto.Price,
          isActive = packageDto.IsActive
        };

        return await _packageRepository.AddAsync(package);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while adding a new package");
        throw;
      }
    }

    public async Task<bool> UpdateAsync(int id, UpdatePackageDto packageDto)
    {
      try
      {
        var package = await _packageRepository.GetByIdAsync(id);
        if (package == null)
        {
          _logger.LogWarning("Package with ID {PackageId} not found", id);
          return false;
        }

        package.Name = packageDto.Name;
        package.Description = packageDto.Description;
        package.Price = packageDto.Price;
        package.isActive = packageDto.IsActive;
        package.UpdateAt = DateTime.UtcNow;

        return await _packageRepository.UpadateAsync(package);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while updating package with ID: {PackageId}", id);
        return false;
      }
    }

    public async Task<bool> DeleteAsync(int id)
    {
      try
      {
        return await _packageRepository.DeleteAsync(id);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while deleting package with ID: {PackageId}", id);
        return false;
      }
    }

    public async Task<bool> TogglePackageStatusAsync(int id)
    {
      try
      {
        var package = await _packageRepository.GetByIdAsync(id);
        if (package == null)
        {
          _logger.LogWarning("Package with ID {PackageId} not found", id);
          return false;
        }

        package.isActive = !package.isActive;
        package.UpdateAt = DateTime.UtcNow;

        return await _packageRepository.UpadateAsync(package);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while toggling package status with ID: {PackageId}", id);
        return false;
      }
    }
  }
}