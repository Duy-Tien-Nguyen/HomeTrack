using HomeTrack.Application.Interface;
using HomeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HomeTrack.Api.Models.Entities;
using System.Threading.Tasks;

namespace HomeTrack.Infrastructure.Repositories
{
  public class PackageRepository : IPackageRepository
  {
    private readonly ApplicationDBContext _context;
    public PackageRepository(ApplicationDBContext context)
    {
      _context = context;
    }
    public async Task<Package?> GetByIdAsync(int Id)
    {
      return await _context.Packages.FindAsync(Id);
    }
    public async Task<IEnumerable<Package>> GetAllAsync()
    {
      return await _context.Packages.ToListAsync();
    }

    public async Task<IEnumerable<Package>> GetActivePackageAsync()
    {
      return await _context.Packages.Where(p => p.isActive).ToListAsync();
    }

    public async Task<Package> AddAsync(Package package)
    {
      await _context.Packages.AddAsync(package);
      return package;
    }

    public async Task<bool> UpdateAsync(Package package)
    {
      _context.Packages.Update(package);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
      var packageToDelete = await _context.Packages.FindAsync(id);
      if (packageToDelete == null)
      {
        return false;
      }
      _context.Packages.Remove(packageToDelete);
      return true;
    }
  }
}