using HomeTrack.Domain;

namespace HomeTrack.Application.Interface
{
  public interface IPackageRepository
  {
    Task<Package?> GetByIdAsync(int id);
    Task<List<Package>?> GetAllAsync();
    Task<Package> AddAsync(Package package);
    Task<bool> UpadateAsync(Package package);
    Task<bool> DeleteAsync(int id);
  }
}