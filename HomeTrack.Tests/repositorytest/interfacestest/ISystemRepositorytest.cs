using System.Collections.Generic;
using System.Threading.Tasks;
using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using Moq;
using Xunit;

namespace HomeTrack.Tests.Repositories
{
    public class ISystemSettingRepositoryTests
    {
        private readonly Mock<ISystemSettingRepository> _mockRepo;
        private readonly SystemSetting _sampleSetting;

        public ISystemSettingRepositoryTests()
        {
            _mockRepo = new Mock<ISystemSettingRepository>();
            _sampleSetting = new SystemSetting { Key = "MaxItems", Value = "100" };
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllSettings()
        {
            var list = new List<SystemSetting> { _sampleSetting };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            var result = await _mockRepo.Object.GetAllAsync();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("MaxItems", result[0].Key);
        }

        [Fact]
        public async Task GetByKeyAsync_ReturnsSetting()
        {
            _mockRepo.Setup(r => r.GetByKeyAsync("MaxItems")).ReturnsAsync(_sampleSetting);

            var result = await _mockRepo.Object.GetByKeyAsync("MaxItems");

            Assert.NotNull(result);
            Assert.Equal("100", result?.Value);
        }

        [Fact]
        public async Task AddAsync_ReturnsTrue()
        {
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<SystemSetting>())).ReturnsAsync(true);

            var result = await _mockRepo.Object.AddAsync(new SystemSetting());

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsTrue()
        {
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<SystemSetting>())).ReturnsAsync(true);

            var result = await _mockRepo.Object.UpdateAsync(_sampleSetting);

            Assert.True(result);
        }
    }
}
