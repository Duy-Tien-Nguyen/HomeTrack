using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using HomeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
  }
}