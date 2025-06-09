using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using HomeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeTrack.Infrastructure.Repositories
{
  public class SystemSettingRepository : ISystemSettingRepository
  {
    private readonly ApplicationDBContext _context;
    private readonly ILogger<SystemSettingRepository> _logger;
    public SystemSettingRepository(ApplicationDBContext context, ILogger<SystemSettingRepository> logger)
    {
      _logger = logger;
      _context = context;
    }
    public async Task<List<SystemSetting>> GetAllAsync()
    {
      try
      {
        return await _context.SystemSettings.AsNoTracking().ToListAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving all system settings");
        return new List<SystemSetting>();
      }
    }

    public async Task<SystemSetting?> GetByKeyAsync(string key)
    {
      return await _context.SystemSettings.FirstOrDefaultAsync(s => s.SettingKey == key);
    }

    public async Task<bool> AddAsync(SystemSetting setting)
    {
      _context.SystemSettings.Add(setting);
      return await _context.SaveChangesAsync() > 0;
    }
    
    public async Task<bool> UpdateAsync(SystemSetting setting)
    {
        // Service đã chuẩn bị entity `setting` với các giá trị mới
        _context.SystemSettings.Update(setting);
        return await _context.SaveChangesAsync() > 0;
    }

  }
}