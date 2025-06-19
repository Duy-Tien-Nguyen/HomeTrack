using System.Collections.Generic;
using System.Threading.Tasks;
using HomeTrack.Api.Request;
using HomeTrack.Application.Interface;
using HomeTrack.Application.Services;
using HomeTrack.Domain;
using HomeTrack.Domain.Enum;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HomeTrack.Tests.Services
{
  public class AdminServiceTests
  {
    private readonly Mock<IUserRepository> _userRepo;
    private readonly Mock<ISystemSettingService> _settingService;
    private readonly Mock<ILogger<AdminService>> _logger;
    private readonly Mock<ApplicationDBContext> _dbContext;
    private readonly AdminService _service;

    public AdminServiceTests()
    {
      _userRepo = new Mock<IUserRepository>();
      _settingService = new Mock<ISystemSettingService>();
      _logger = new Mock<ILogger<AdminService>>();
      _dbContext = new Mock<ApplicationDBContext>();
      _service = new AdminService(_settingService.Object, _logger.Object, _userRepo.Object, _dbContext.Object);
    }

    [Fact]
    public async Task ViewUserDetail_ReturnsDto_WhenUserExists()
    {
      var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Role = Role.Basic };
      _userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

      var result = await _service.ViewUserDetail(1);

      Assert.NotNull(result);
      Assert.Equal("John", result.FirstName);
    }

    [Fact]
    public async Task GetAllUser_ReturnsList_WhenUsersExist()
    {
      var users = new List<User>
      {
        new User { Id = 1, FirstName = "A", LastName = "B", Email = "a@b.com", Role = Role.Basic }
      };
      _userRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

      var result = await _service.GetAllUser();

      Assert.NotNull(result);
      Assert.Single(result);
    }

    [Fact]
    public async Task ChangeItemLimit_ReturnsTrue_WhenValid()
    {
      var setting = new SystemSetting { SettingKey = "MaxBasicItemLimit", SettingValue = 10 };
      _settingService.Setup(s => s.GetByKeyAsync("MaxBasicItemLimit")).ReturnsAsync(setting);
      _dbContext.Setup(d => d.SaveChangesAsync(default)).ReturnsAsync(1);

      var result = await _service.ChangeItemLimit(PackageType.Basic, 20);

      Assert.True(result);
    }

    [Fact]
    public async Task UpgradeDowngrade_ReturnsTrue_WhenUserExists()
    {
      var user = new User { Id = 1, Role = Role.Basic };
      _userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
      _userRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

      var result = await _service.UpgradeDowngrade(1, PackageType.Premium);

      Assert.True(result);
      Assert.Equal(Role.Premium, user.Role);
    }

    [Fact]
    public async Task BanUnlock_ReturnsTrue_WhenUserExists()
    {
      var user = new User { Id = 1, Status = UserStatus.Active };
      _userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
      _userRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

      var result = await _service.BanUnlock(1, UserStatus.Banned);

      Assert.True(result);
      Assert.Equal(UserStatus.Banned, user.Status);
    }
  }
}