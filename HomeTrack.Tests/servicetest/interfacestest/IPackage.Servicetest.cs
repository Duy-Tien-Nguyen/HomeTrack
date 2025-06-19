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
    private readonly Mock<IPackageRepository> _packageRepo;
    private readonly Mock<ISubscriptionRepository> _subscriptionRepo;
    private readonly Mock<IUserRepository> _userRepo;
    private readonly Mock<ILogger<PackageService>> _logger;
    private readonly PackageService _service;

    public PackageServiceTests()
    {
      _packageRepo = new Mock<IPackageRepository>();
      _subscriptionRepo = new Mock<ISubscriptionRepository>();
      _userRepo = new Mock<IUserRepository>();
      _logger = new Mock<ILogger<PackageService>>();

      _service = new PackageService(_subscriptionRepo.Object, _packageRepo.Object, _userRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsPackage()
    {
      _packageRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Package { Id = 1 });

      var result = await _service.GetByIdAsync(1);

      Assert.NotNull(result);
      Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsList()
    {
      _packageRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Package> { new Package() });

      var result = await _service.GetAllAsync();

      Assert.NotNull(result);
      Assert.Single(result);
    }

    [Fact]
    public async Task AddAsync_AddsPackageSuccessfully()
    {
      var dto = new CreatePackageDto { Name = "Basic", Description = "desc", DurationDays = 30, Price = 10, IsActive = true };
      var package = new Package { Name = dto.Name, Description = dto.Description, DurationDays = dto.DurationDays, Price = dto.Price, isActive = dto.IsActive };
      _packageRepo.Setup(r => r.AddAsync(It.IsAny<Package>())).ReturnsAsync(package);

      var result = await _service.AddAsync(dto);

      Assert.Equal(dto.Name, result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ValidId_ReturnsTrue()
    {
      var existing = new Package { Id = 1 };
      var dto = new UpdatePackageDto { Name = "Updated", Description = "Desc", DurationDays = 15, Price = 20, IsActive = false };

      _packageRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
      _packageRepo.Setup(r => r.UpadateAsync(It.IsAny<Package>())).ReturnsAsync(true);

      var result = await _service.UpdateAsync(1, dto);

      Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ValidId_ReturnsTrue()
    {
      _packageRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

      var result = await _service.DeleteAsync(1);

      Assert.True(result);
    }

    [Fact]
    public async Task TogglePackageStatusAsync_FlipsStatus()
    {
      var pkg = new Package { Id = 1, isActive = true };
      _packageRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pkg);
      _packageRepo.Setup(r => r.UpadateAsync(pkg)).ReturnsAsync(true);

      var result = await _service.TogglePackageStatusAsync(1);

      Assert.True(result);
      Assert.False(pkg.isActive);
    }
  }
}