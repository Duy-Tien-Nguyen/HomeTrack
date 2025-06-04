using HomeTrack.Domain;
using HomeTrack.Api.Request;

namespace HomeTrack.Application.Interface
{
  public interface IPackageService
  {
    Task<Package?> GetByIdAsync(int id);
    Task<List<Package>?> GetAllAsync();
    Task<Package> AddAsync(CreatePackageDto packageDto);
    Task<bool> UpdateAsync(int id, UpdatePackageDto packageDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> TogglePackageStatusAsync(int id);
  }
}