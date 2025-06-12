using Xunit;
using Moq;
using HomeTrack.Api.Controllers;
using HomeTrack.Application.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using HomeTrack.Api.Request;
using HomeTrack.Domain.Entity;

namespace HomeTrack.Tests.Controllers
{
    public class PackageControllerTests
    {
        private readonly Mock<IPackageService> _mockService;
        private readonly PackageController _controller;

        public PackageControllerTests()
        {
            _mockService = new Mock<IPackageService>();
            _controller = new PackageController(_mockService.Object);
        }

        [Fact]
        public async Task GetPackageById_ReturnsNotFound_WhenPackageIsNull()
        {
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Package)null);

            var result = await _controller.GetPackageById(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAllPackages_ReturnsOk_WhenPackagesExist()
        {
            var fakePackages = new List<Package> { new Package { Id = 1 } };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(fakePackages);

            var result = await _controller.GetAllPackages();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(fakePackages, okResult.Value);
        }

        [Fact]
        public async Task CreatePackage_ReturnsCreatedAt_WhenSuccess()
        {
            var dto = new CreatePackageDto { Name = "Test" };
            var createdPackage = new Package { Id = 1, Name = "Test" };

            _mockService.Setup(s => s.AddAsync(dto)).ReturnsAsync(createdPackage);

            var result = await _controller.CreatePackage(dto);

            var createdAt = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(createdPackage, createdAt.Value);
        }

        [Fact]
        public async Task UpdatePackage_ReturnsOk_WhenUpdated()
        {
            var dto = new UpdatePackageDto { Name = "Updated" };
            _mockService.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(true);

            var result = await _controller.UpdatePackage(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Gói đã được cập nhật thành công.", ok.Value);
        }

        [Fact]
        public async Task DeletePackage_ReturnsNoContent_WhenDeleted()
        {
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeletePackage(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task TogglePackageStatus_ReturnsOk_WhenToggled()
        {
            _mockService.Setup(s => s.TogglePackageStatusAsync(1)).ReturnsAsync(true);

            var result = await _controller.TogglePackageStatus(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Trạng thái gói đã được thay đổi thành công.", ok.Value);
        }
    }
}
