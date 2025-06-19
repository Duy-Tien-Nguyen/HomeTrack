using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeTrack.Application.Interface;
using HomeTrack.Application.Services;
using HomeTrack.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HomeTrack.Tests.Services
{
  public class SystemSettingServiceTests
  {
    private readonly Mock<ISystemSettingRepository> _settingRepo;
    private readonly Mock<ILogger<SystemSettingService>> _logger;
    private readonly SystemSettingService _service;

    public SystemSettingServiceTests()
    {
      _settingRepo = new Mock<ISystemSettingRepository>();
      _logger = new Mock<ILogger<SystemSettingService>>();
      _service = new SystemSettingService(_settingRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsList()
    {
      var settings = new List<SystemSetting> { new SystemSetting { SettingKey = "Test", SettingValue = 1 } };
      _settingRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(settings);

      var result = await _service.GetAllAsync();

      Assert.NotNull(result);
      Assert.Single(result);
    }

    [Fact]
    public async Task GetByKeyAsync_ReturnsSetting()
    {
      var setting = new SystemSetting { SettingKey = "Limit", SettingValue = 100 };
      _settingRepo.Setup(r => r.GetByKeyAsync("Limit")).ReturnsAsync(setting);

      var result = await _service.GetByKeyAsync("Limit");

      Assert.NotNull(result);
      Assert.Equal("Limit", result.SettingKey);
    }

    [Fact]
    public async Task AddAsync_ReturnsTrue()
    {
      var setting = new SystemSetting { SettingKey = "NewSetting", SettingValue = 5 };
      _settingRepo.Setup(r => r.AddAsync(setting)).ReturnsAsync(true);

      var result = await _service.AddAsync(setting);

      Assert.True(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue()
    {
      var setting = new SystemSetting { SettingKey = "UpdateSetting", SettingValue = 10 };
      _settingRepo.Setup(r => r.UpdateAsync(setting)).ReturnsAsync(true);

      var result = await _service.UpdateAsync(setting);

      Assert.True(result);
    }
  }
}
