using HomeTrack.Domain;

namespace HomeTrack.Application.Interface
{
  public interface ISystemSettingRepository
  {
    Task<List<SystemSetting>> GetAllAsync(); // Trả về List<SystemSetting>
    Task<SystemSetting?> GetByKeyAsync(string key); // Trả về SystemSetting?
    Task<bool> AddAsync(SystemSetting setting);
    Task<bool> UpdateAsync(SystemSetting setting);
    // Task<bool> DeleteAsync(string key); 
  }
}