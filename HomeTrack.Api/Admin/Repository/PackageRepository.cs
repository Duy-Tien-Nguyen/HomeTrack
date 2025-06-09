using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using HomeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeTrack.Infrastructure.Repositories
{
  public class PackageRepository : IPackageRepository
  {
    private readonly ApplicationDBContext _context;
    private readonly ILogger<PackageRepository> _logger;

    public PackageRepository(ApplicationDBContext context, ILogger<PackageRepository> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task<Package?> GetByIdAsync(int id)
    {
      try
      {
        return await _context.Packages.FindAsync(id);
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
        return await _context.Packages.ToListAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while getting all packages");
        return new List<Package>();
      }
    }

    public async Task<Package> AddAsync(Package package)
    {
      try
      {
        package.CreateAt = DateTime.UtcNow;
        package.UpdateAt = DateTime.UtcNow;

        _context.Packages.Add(package);
        await _context.SaveChangesAsync();

        return package;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while adding a new package");
        throw;
      }
    }

    public async Task<bool> UpadateAsync(Package package)
    {
      try
      {
        var existingPackage = await _context.Packages.FindAsync(package.Id);
        if (existingPackage == null)
        {
          _logger.LogWarning("Package with ID {PackageId} not found for update", package.Id);
          return false;
        }

        existingPackage.Name = package.Name;
        existingPackage.Description = package.Description;
        existingPackage.Price = package.Price;
        existingPackage.DurationDays = package.DurationDays;
        existingPackage.isActive = package.isActive;
        existingPackage.UpdateAt = DateTime.UtcNow;

        _context.Packages.Update(existingPackage);
        await _context.SaveChangesAsync();

        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while updating package with ID: {PackageId}", package.Id);
        return false;
      }
    }

    public async Task<bool> DeleteAsync(int id)
    {
      try
      {
        var package = await _context.Packages.FindAsync(id);
        if (package == null)
        {
          _logger.LogWarning("Package with ID {PackageId} not found for deletion", id);
          return false;
        }

        _context.Packages.Remove(package);
        await _context.SaveChangesAsync(); 
        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while deleting package with ID: {PackageId}", id);
        return false;
      }
    }
  }
}