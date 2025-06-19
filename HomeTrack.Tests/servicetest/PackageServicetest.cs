
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeTrack.Api.Request;
using HomeTrack.Application.Interface;
using HomeTrack.Application.Services;
using HomeTrack.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HomeTrack.Tests.Services
{
    public class PackageServiceTests
    {
        private readonly Mock<IPackageRepository> _packageRepoMock;
        private readonly Mock<ISubscriptionRepository> _subscriptionRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ILogger<PackageService>> _loggerMock;
        private readonly PackageService _service;

        public PackageServiceTests()
        {
            _packageRepoMock = new Mock<IPackageRepository>();
            _subscriptionRepoMock = new Mock<ISubscriptionRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<PackageService>>();

            _service = new PackageService(
                _subscriptionRepoMock.Object,
                _packageRepoMock.Object,
                _userRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsPackage_WhenFound()
        {
            var package = new Package { Id = 1 };
            _packageRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(package);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result?.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _packageRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Package?)null);

            var result = await _service.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfPackages()
        {
            _packageRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Package> { new Package { Id = 1 } });

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Single(result!);
        }

        [Fact]
        public async Task AddAsync_CreatesNewPackage()
        {
            var dto = new CreatePackageDto
            {
                Name = "Test",
                Description = "Test",
                Price = 10,
                DurationDays = 30,
                IsActive = true
            };

            _packageRepoMock.Setup(r => r.AddAsync(It.IsAny<Package>())).ReturnsAsync((Package p) => p);

            var result = await _service.AddAsync(dto);

            Assert.Equal("Test", result.Name);
            Assert.True(result.isActive);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesPackage_WhenExists()
        {
            var package = new Package { Id = 1 };
            var dto = new UpdatePackageDto
            {
                Name = "Updated",
                Description = "Updated Desc",
                Price = 100,
                DurationDays = 60,
                IsActive = false
            };

            _packageRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(package);
            _packageRepoMock.Setup(r => r.UpadateAsync(package)).ReturnsAsync(true);

            var result = await _service.UpdateAsync(1, dto);

            Assert.True(result);
            Assert.Equal("Updated", package.Name);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_WhenPackageNotFound()
        {
            _packageRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Package?)null);

            var result = await _service.UpdateAsync(999, new UpdatePackageDto());

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenDeleted()
        {
            _packageRepoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_OnError()
        {
            _packageRepoMock.Setup(r => r.DeleteAsync(1)).ThrowsAsync(new Exception("error"));

            var result = await _service.DeleteAsync(1);

            Assert.False(result);
        }

        [Fact]
        public async Task TogglePackageStatusAsync_TogglesStatus_WhenFound()
        {
            var package = new Package { Id = 1, isActive = true };
            _packageRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(package);
            _packageRepoMock.Setup(r => r.UpadateAsync(package)).ReturnsAsync(true);

            var result = await _service.TogglePackageStatusAsync(1);

            Assert.True(result);
            Assert.False(package.isActive); // status flipped
        }

        [Fact]
        public async Task TogglePackageStatusAsync_ReturnsFalse_WhenNotFound()
        {
            _packageRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Package?)null);

            var result = await _service.TogglePackageStatusAsync(999);

            Assert.False(result);
        }
    }
}
