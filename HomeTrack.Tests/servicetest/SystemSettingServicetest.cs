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
      _settingRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<SystemSetting> { new SystemSetting { SettingKey = "MaxLimit" } });

      var result = await _service.GetAllAsync();

      Assert.NotNull(result);
      Assert.Single(result);
    }

    [Fact]
    public async Task GetByKeyAsync_ValidKey_ReturnsSetting()
    {
      _settingRepo.Setup(r => r.GetByKeyAsync("MaxLimit"))
        .ReturnsAsync(new SystemSetting { SettingKey = "MaxLimit", SettingValue = 10 });

      var result = await _service.GetByKeyAsync("MaxLimit");

      Assert.NotNull(result);
      Assert.Equal("MaxLimit", result?.SettingKey);
    }

    [Fact]
    public async Task GetByKeyAsync_EmptyKey_ReturnsNull()
    {
      var result = await _service.GetByKeyAsync("");
      Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ValidSetting_ReturnsTrue()
    {
      var setting = new SystemSetting { SettingKey = "MaxLimit", SettingValue = 20 };
      _settingRepo.Setup(r => r.AddAsync(setting)).ReturnsAsync(true);

      var result = await _service.AddAsync(setting);

      Assert.True(result);
    }

    [Fact]
    public async Task AddAsync_NullSetting_ReturnsFalse()
    {
      var result = await _service.AddAsync(null);
      Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ValidSetting_ReturnsTrue()
    {
      var setting = new SystemSetting { SettingKey = "MaxLimit", SettingValue = 50 };
      _settingRepo.Setup(r => r.UpdateAsync(setting)).ReturnsAsync(true);

      var result = await _service.UpdateAsync(setting);

      Assert.True(result);
    }

    [Fact]
    public async Task UpdateAsync_NullSetting_ReturnsFalse()
    {
      var result = await _service.UpdateAsync(null);
      Assert.False(result);
    }
  }
}
