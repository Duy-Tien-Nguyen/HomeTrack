using HomeTrack.Domain;

namespace HomeTrack.Application.Interface
{
  public interface IPackageRepository
  {
    Task<Package?> GetByIdAsync(int Id);
    Task<IEnumerable<Package>> GetAllAsync();
    Task<IEnumerable<Package>> GetActivePackageAsync();
    Task<Package> AddAsync(Package package);
    Task<bool> UpadateAsync(Package package);
    Task<bool> DeleteAsync(int id);
  }
}