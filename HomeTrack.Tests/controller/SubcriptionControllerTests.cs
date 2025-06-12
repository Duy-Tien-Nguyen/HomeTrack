using System.Security.Claims;
using HomeTrack.Api.Controllers;
using HomeTrack.Api.Request;
using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class SubscriptionControllerTests
{
    private readonly Mock<ISubscriptionService> _mockSubscriptionService;
    private readonly Mock<IPackageService> _mockPackageService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly SubscriptionController _controller;

    public SubscriptionControllerTests()
    {
        _mockSubscriptionService = new Mock<ISubscriptionService>();
        _mockPackageService = new Mock<IPackageService>();
        _mockUserRepository = new Mock<IUserRepository>();

        _controller = new SubscriptionController(
            _mockSubscriptionService.Object,
            _mockPackageService.Object,
            _mockUserRepository.Object
        );

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }

    [Fact]
    public async Task GetByMyself_ReturnsOk_WhenSubscriptionsExist()
    {
        // Arrange
        var subs = new List<Subscription> { new Subscription { Id = 1, UserId = 1 } };
        _mockSubscriptionService.Setup(s => s.GetByUserIdAsync(1)).ReturnsAsync(subs);

        // Act
        var result = await _controller.GetByMyself();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsAssignableFrom<IEnumerable<Subscription>>(okResult.Value);
        Assert.Single(data);
    }

    [Fact]
    public async Task CancelSubs_ReturnsOk_WhenCancellationSucceeds()
    {
        // Arrange
        var req = new GetSubscriptionByIdReq { subcriptionId = 1 };
        var subs = new Subscription { Id = 1, UserId = 1 };
        _mockSubscriptionService.Setup(s => s.CancelAsync(1, 1)).ReturnsAsync(subs);

        // Act
        var result = await _controller.CancelSubs(req);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedSub = Assert.IsType<Subscription>(okResult.Value);
        Assert.Equal(1, returnedSub.Id);
    }

    [Fact]
    public async Task RegisAsync_ReturnsOk_WhenSubscriptionCreated()
    {
        // Arrange
        var dto = new CreateSubscriptionDto { UserId = 1, PackageId = 1 };
        var subs = new Subscription { Id = 1, UserId = 1 };
        _mockSubscriptionService.Setup(s => s.AddAsync(dto)).ReturnsAsync(subs);

        // Act
        var result = await _controller.RegisAsync(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedSub = Assert.IsType<Subscription>(okResult.Value);
        Assert.Equal(1, returnedSub.UserId);
    }
}

