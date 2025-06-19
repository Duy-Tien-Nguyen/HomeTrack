using System.Collections.Generic;
using System.Threading.Tasks;
using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using Moq;
using Xunit;

namespace HomeTrack.Tests.Repositories
{
    public class IPackageRepositoryTests
    {
        private readonly Mock<IPackageRepository> _mockRepo;
        private readonly Package _samplePackage;

        public IPackageRepositoryTests()
        {
            _mockRepo = new Mock<IPackageRepository>();
            _samplePackage = new Package { Id = 1, Name = "Premium" };
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsPackage()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(_samplePackage);

            var result = await _mockRepo.Object.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Premium", result?.Name);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfPackages()
        {
            var list = new List<Package> { _samplePackage };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            var result = await _mockRepo.Object.GetAllAsync();

            Assert.NotNull(result);
            Assert.Single(result!);
        }

        [Fact]
        public async Task AddAsync_ReturnsCreatedPackage()
        {
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Package>())).ReturnsAsync(_samplePackage);

            var result = await _mockRepo.Object.AddAsync(new Package());

            Assert.NotNull(result);
            Assert.Equal(_samplePackage.Id, result.Id);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsTrue()
        {
            _mockRepo.Setup(r => r.UpadateAsync(It.IsAny<Package>())).ReturnsAsync(true);

            var result = await _mockRepo.Object.UpadateAsync(_samplePackage);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue()
        {
            _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _mockRepo.Object.DeleteAsync(1);

            Assert.True(result);
        }
    }
}
