namespace HomeTrack.Application.Interface
{
  public interface ISystemSettingService
  {
    Task<List<SystemSetting>> GetAllAsync(); // Trả về List<SystemSetting>
    Task<SystemSetting?> GetByKeyAsync(string key); // Trả về SystemSetting?
    Task<bool> AddAsync(SystemSetting setting);
    Task<bool> UpdateAsync(SystemSetting setting);
  }
}