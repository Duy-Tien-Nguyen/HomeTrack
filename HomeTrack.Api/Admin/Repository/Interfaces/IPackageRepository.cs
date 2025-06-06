using HomeTrack.Application.Interface;
using HomeTrack.Domain; 
using Microsoft.EntityFrameworkCore;

namespace HomeTrack.Infrastructure.Repositories
{
  public interface IPackageRepository
  {
    Task<Package?> GetByIdAsync(int id);
    Task<List<Package>?> GetAllAsync();
    Task<Package> AddAsync(Package package);
    Task<bool> UpdateAsync(Package package);
    Task<bool> DeleteAsync(int id);
  }
}
