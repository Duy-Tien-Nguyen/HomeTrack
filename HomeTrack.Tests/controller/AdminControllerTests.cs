using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeTrack.Api.Controllers;
using HomeTrack.Application.Interface;
using HomeTrack.Domain.Enum;
using System.Collections.Generic;

namespace HomeTrack.Tests
{
    public class AdminControllerTests
    {
        private readonly Mock<IAdminService> _adminServiceMock;
        private readonly Mock<ISystemSettingRepository> _settingRepoMock;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _adminServiceMock = new Mock<IAdminService>();
            _settingRepoMock = new Mock<ISystemSettingRepository>();
            _controller = new AdminController(_adminServiceMock.Object, _settingRepoMock.Object);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk_WhenUsersExist()
        {
            // Arrange
            _adminServiceMock.Setup(s => s.GetAllUser()).ReturnsAsync(new List<object> { new object() });

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task ViewUserDetail_ReturnsNotFound_IfUserIsNull()
        {
            // Arrange
            int userId = 1;
            _adminServiceMock.Setup(s => s.ViewUserDetail(userId)).ReturnsAsync((object)null);

            // Act
            var result = await _controller.ViewUserDetail(userId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpgradeUser_ReturnsOk_IfSuccess()
        {
            _adminServiceMock.Setup(s => s.UpgradeDowngrade(1, PackageType.Premium)).ReturnsAsync(true);

            var result = await _controller.UpgradeUser(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DowngradeUser_ReturnsBadRequest_IfSelfDowngrade()
        {
            // Setup Claims
            var controllerContext = new ControllerContext();
            _controller.ControllerContext = controllerContext;

            var mockUser = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "2")
                })
            );
            _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = mockUser
            };

            var result = await _controller.DowngradeUser(2);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task BanUser_ReturnsOk_IfSuccess()
        {
            _adminServiceMock.Setup(s => s.BanUnlock(5, UserStatus.Banned)).ReturnsAsync(true);

            var result = await _controller.BanUser(5);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetAllPackages_ReturnsNotFound_IfEmpty()
        {
            _settingRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<object>());

            var result = await _controller.GetAllPackages();

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
