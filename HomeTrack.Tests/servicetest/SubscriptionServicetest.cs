using System;
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
  public class SubscriptionServiceTests
  {
    private readonly Mock<ISubscriptionRepository> _subscriptionRepo;
    private readonly Mock<IPackageRepository> _packageRepo;
    private readonly Mock<IUserRepository> _userRepo;
    private readonly Mock<ILogger<SubscriptionService>> _logger;
    private readonly SubscriptionService _service;

    public SubscriptionServiceTests()
    {
      _subscriptionRepo = new Mock<ISubscriptionRepository>();
      _packageRepo = new Mock<IPackageRepository>();
      _userRepo = new Mock<IUserRepository>();
      _logger = new Mock<ILogger<SubscriptionService>>();
      _service = new SubscriptionService(_subscriptionRepo.Object, _packageRepo.Object, _userRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsSubscription()
    {
      var subscription = new Subscription { Id = 1 };
      _subscriptionRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(subscription);

      var result = await _service.GetByIdAsync(1);

      Assert.NotNull(result);
      Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSubscriptions()
    {
      _subscriptionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Subscription> { new Subscription { Id = 1 } });

      var result = await _service.GetAllAsync();

      Assert.Single(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsUserSubscriptions()
    {
      _subscriptionRepo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(new List<Subscription> { new Subscription { UserId = 1 } });

      var result = await _service.GetByUserIdAsync(1);

      Assert.Single(result);
    }

    [Fact]
    public async Task GetActiveSubscriptionByUserIdAsync_ReturnsSubscription()
    {
      var sub = new Subscription { UserId = 1, PackageId = 1, Status = SubscriptionStatus.Active };
      _subscriptionRepo.Setup(r => r.GetActiveSubscriptionByUserIdAsync(1, 1)).ReturnsAsync(sub);

      var result = await _service.GetActiveSubscriptionByUserIdAsync(1, 1);

      Assert.NotNull(result);
      Assert.Equal(SubscriptionStatus.Active, result?.Status);
    }

    [Fact]
    public async Task AddAsync_ShouldCreateSubscription()
    {
      var dto = new CreateSubscriptionDto { UserId = 1, PackageId = 1 };
      var user = new User { Id = 1, FirstName = "John", LastName = "Doe" };
      var package = new Package { Id = 1, Name = "Basic", DurationDays = 30, Price = 100 };

      _userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
      _packageRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(package);
      _subscriptionRepo.Setup(r => r.GetActiveSubscriptionByUserIdAsync(1, 1)).ReturnsAsync((Subscription)null);
      _subscriptionRepo.Setup(r => r.DeleteAllByUser(1)).ReturnsAsync(true);
      _subscriptionRepo.Setup(r => r.AddAsync(It.IsAny<Subscription>())).ReturnsAsync((Subscription s) => s);

      var result = await _service.AddAsync(dto);

      Assert.NotNull(result);
      Assert.Equal("Basic", result.PackageName);
      Assert.Equal("John Doe", result.UserName);
    }

    [Fact]
    public async Task CancelAsync_ShouldCancelSubscription()
    {
      var subscription = new Subscription { Id = 1, UserId = 1 };
      _subscriptionRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(subscription);
      _subscriptionRepo.Setup(r => r.UpdateAsync(It.IsAny<Subscription>())).Returns(Task.CompletedTask);

      var result = await _service.CancelAsync(1, 1);

      Assert.Equal(SubscriptionStatus.Cancelled, result.Status);
    }

    [Fact]
    public async Task ExpireAsync_ShouldExpireSubscription()
    {
      var subscription = new Subscription { Id = 1, UserId = 1 };
      _subscriptionRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(subscription);
      _subscriptionRepo.Setup(r => r.UpdateAsync(It.IsAny<Subscription>())).Returns(Task.CompletedTask);

      var result = await _service.ExpireAsync(1, 1);

      Assert.Equal(SubscriptionStatus.Expired, result.Status);
    }

    [Fact]
    public async Task ActivateAsync_ShouldSetToActive()
    {
      var subscription = new Subscription { Id = 1, UserId = 1 };
      _subscriptionRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(subscription);
      _subscriptionRepo.Setup(r => r.UpdateAsync(It.IsAny<Subscription>())).Returns(Task.CompletedTask);

      var result = await _service.ActivateAsync(1, 1);

      Assert.True(result);
    }
  }
}
