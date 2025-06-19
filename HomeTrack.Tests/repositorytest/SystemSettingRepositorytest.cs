using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeTrack.Domain;
using HomeTrack.Infrastructure.Data;
using HomeTrack.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HomeTrack.Tests.Repositories
{
    public class SystemSettingRepositoryTests
    {
        private readonly ApplicationDBContext _context;
        private readonly SystemSettingRepository _repository;
        private readonly Mock<ILogger<SystemSettingRepository>> _loggerMock;

        public SystemSettingRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Mỗi test dùng DB riêng biệt
                .Options;

            _context = new ApplicationDBContext(options);
            _loggerMock = new Mock<ILogger<SystemSettingRepository>>();
            _repository = new SystemSettingRepository(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddSetting()
        {
            var setting = new SystemSetting { SettingKey = "SiteName", SettingValue = "HomeTrack" };

            var result = await _repository.AddAsync(setting);

            Assert.True(result);
            Assert.Single(_context.SystemSettings);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllSettings()
        {
            _context.SystemSettings.AddRange(
                new SystemSetting { SettingKey = "A", SettingValue = "1" },
                new SystemSetting { SettingKey = "B", SettingValue = "2" });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByKeyAsync_ShouldReturnCorrectSetting()
        {
            var setting = new SystemSetting { SettingKey = "Theme", SettingValue = "Dark" };
            _context.SystemSettings.Add(setting);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByKeyAsync("Theme");

            Assert.NotNull(result);
            Assert.Equal("Dark", result?.SettingValue);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingSetting()
        {
            var setting = new SystemSetting { SettingKey = "Version", SettingValue = "1.0" };
            _context.SystemSettings.Add(setting);
            await _context.SaveChangesAsync();

            setting.SettingValue = "2.0";
            var result = await _repository.UpdateAsync(setting);

            Assert.True(result);
            var updated = await _context.SystemSettings.FirstOrDefaultAsync(s => s.SettingKey == "Version");
            Assert.Equal("2.0", updated?.SettingValue);
        }
    }
}
