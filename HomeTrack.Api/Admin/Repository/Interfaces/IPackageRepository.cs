using HomeTrack.Application.Interface;
// using HomeTrack.Domain; // <-- Xóa hoặc comment
using HomeTrack.Api.Models.Entities; // <-- Đảm bảo dòng này
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
