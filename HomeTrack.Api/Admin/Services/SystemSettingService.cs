using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HomeTrack.Application.Services
{
  public class SystemSettingService : ISystemSettingService
  {
    private readonly ISystemSettingRepository _systemSettingRepository;
    private readonly ILogger<SystemSettingService> _logger;

    public SystemSettingService(ISystemSettingRepository systemSettingRepository, ILogger<SystemSettingService> logger)
    {
      _systemSettingRepository = systemSettingRepository;
      _logger = logger;
    }

    public async Task<List<SystemSetting>?> GetAllAsync()
    {
      try
      {
        return await _systemSettingRepository.GetAllAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while getting all system settings");
        return new List<SystemSetting>();
      }
    }

    public async Task<SystemSetting?> GetByKeyAsync(string key)
    {
      if (string.IsNullOrWhiteSpace(key))
      {
        _logger.LogWarning("GetByKeyAsync called with a null or whitespace key.");
        return null;
      }
      try
      {
        var setting = await _systemSettingRepository.GetByKeyAsync(key);
        if (setting == null)
        {
          _logger.LogInformation("System setting with key: {Key} not found.", key);
        }
        else
        {
          _logger.LogInformation("System setting with key: {Key} found.", key);
        }
        return setting;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while getting system setting by key: {Key}", key);
        return null;
      }
    }

    public async Task<bool> AddAsync(SystemSetting setting)
    {
      if (setting == null)
      {
        _logger.LogWarning("AddAsync called with a null setting.");
        return false;
      }
      try
      {
        return await _systemSettingRepository.AddAsync(setting);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while adding system setting: {SettingKey}", setting.SettingKey);
        return false;
      }
    }

    public async Task<bool> UpdateAsync(SystemSetting setting)
    {
      if (setting == null)
      {
        _logger.LogWarning("UpdateAsync called with a null setting.");
        return false;
      }
      try
      {
        return await _systemSettingRepository.UpdateAsync(setting);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while updating system setting: {SettingKey}", setting.SettingKey);
        return false;
      }
    }
  }
}