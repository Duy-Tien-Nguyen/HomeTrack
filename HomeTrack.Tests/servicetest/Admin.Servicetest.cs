using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeTrack.Api.Request;
using HomeTrack.Application.Interface;
using HomeTrack.Application.Services;
using HomeTrack.Domain;
using HomeTrack.Domain.Enum;
using HomeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HomeTrack.Tests.Services
{
    public class AdminServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ISystemSettingService> _settingServiceMock;
        private readonly Mock<ILogger<AdminService>> _loggerMock;
        private readonly ApplicationDBContext _context;
        private readonly AdminService _service;

        public AdminServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "AdminServiceTests")
                .Options;
            _context = new ApplicationDBContext(options);

            _userRepoMock = new Mock<IUserRepository>();
            _settingServiceMock = new Mock<ISystemSettingService>();
            _loggerMock = new Mock<ILogger<AdminService>>();

            _service = new AdminService(_settingServiceMock.Object, _loggerMock.Object, _userRepoMock.Object, _context);
        }

        [Fact]
        public async Task ViewUserDetail_ReturnsUserDto_WhenUserExists()
        {
            var user = new User { Id = 1, FirstName = "A", LastName = "B", Role = Role.Basic, Email = "a@b.com" };
            _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            var result = await _service.ViewUserDetail(1);

            Assert.NotNull(result);
            Assert.Equal("A", result.FirstName);
        }

        [Fact]
        public async Task ViewUserDetail_ReturnsNull_WhenUserNotFound()
        {
            _userRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

            var result = await _service.ViewUserDetail(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllUser_ReturnsListOfUsers()
        {
            var users = new List<User> {
                new User { Id = 1, FirstName = "A", LastName = "B", Role = Role.Premium, Email = "a@b.com" }
            };
            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            var result = await _service.GetAllUser();

            Assert.Single(result);
        }

        [Fact]
        public async Task ChangeItemLimit_UpdatesSetting_WhenValid()
        {
            var setting = new SystemSetting { SettingKey = "MaxPremiumItemLimit", SettingValue = "10" };
            _settingServiceMock.Setup(s => s.GetByKeyAsync("MaxPremiumItemLimit")).ReturnsAsync(setting);

            var result = await _service.ChangeItemLimit(PackageType.Premium, 50);

            Assert.True(result);
            Assert.Equal("50", setting.SettingValue);
        }

        [Fact]
        public async Task UpgradeDowngrade_ChangesRole_WhenValid()
        {
            var user = new User { Id = 1, Role = Role.Basic };
            _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.UpgradeDowngrade(1, PackageType.Premium);

            Assert.True(result);
            Assert.Equal(Role.Premium, user.Role);
        }

        [Fact]
        public async Task BanUnlock_UpdatesStatus_WhenUserExists()
        {
            var user = new User { Id = 2, Status = UserStatus.Active };
            _userRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.BanUnlock(2, UserStatus.Banned);

            Assert.True(result);
            Assert.Equal(UserStatus.Banned, user.Status);
        }
    }
}
