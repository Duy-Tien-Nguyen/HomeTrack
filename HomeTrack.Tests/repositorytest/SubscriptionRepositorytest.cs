using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeTrack.Domain;
using HomeTrack.Domain.Enums;
using HomeTrack.Infrastructure.Data;
using HomeTrack.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HomeTrack.Tests.Repositories
{
    public class SubscriptionRepositoryTests
    {
        private readonly ApplicationDBContext _context;
        private readonly SubscriptionRepository _repository;
        private readonly Mock<ILogger<SubscriptionRepository>> _loggerMock;

        public SubscriptionRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // DB tạm thời, riêng mỗi test
                .Options;

            _context = new ApplicationDBContext(options);
            _loggerMock = new Mock<ILogger<SubscriptionRepository>>();
            _repository = new SubscriptionRepository(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddAndReturnSubscription()
        {
            var sub = new Subscription { UserId = 1, PackageId = 1, Status = SubscriptionStatus.Active };

            var result = await _repository.AddAsync(sub);

            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.NotEqual(default, result.CreatedAt);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsSubscription_WhenExists()
        {
            var sub = new Subscription { UserId = 1, PackageId = 1 };
            _context.Subscriptions.Add(sub);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(sub.Id);

            Assert.NotNull(result);
            Assert.Equal(1, result?.UserId);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllSubscriptions()
        {
            _context.Subscriptions.AddRange(
                new Subscription { UserId = 1, PackageId = 1 },
                new Subscription { UserId = 2, PackageId = 2 });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsSubscriptions()
        {
            _context.Subscriptions.Add(new Subscription { UserId = 99, PackageId = 1 });
            await _context.SaveChangesAsync();

            var result = await _repository.GetByUserIdAsync(99);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetActiveSubscriptionByUserIdAsync_ReturnsActive()
        {
            _context.Subscriptions.Add(new Subscription
            {
                UserId = 3,
                PackageId = 5,
                Status = SubscriptionStatus.Active
            });
            await _context.SaveChangesAsync();

            var result = await _repository.GetActiveSubscriptionByUserIdAsync(3, 5);

            Assert.NotNull(result);
            Assert.Equal(SubscriptionStatus.Active, result?.Status);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateSubscription()
        {
            var sub = new Subscription { UserId = 10, PackageId = 2, Status = SubscriptionStatus.Pending };
            _context.Subscriptions.Add(sub);
            await _context.SaveChangesAsync();

            sub.Status = SubscriptionStatus.Active;
            await _repository.UpdateAsync(sub);

            var updated = await _context.Subscriptions.FindAsync(sub.Id);
            Assert.Equal(SubscriptionStatus.Active, updated?.Status);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenExists()
        {
            var sub = new Subscription { UserId = 1, PackageId = 1 };
            _context.Subscriptions.Add(sub);
            await _context.SaveChangesAsync();

            var exists = await _repository.ExistsAsync(sub.Id);

            Assert.True(exists);
        }

        [Fact]
        public async Task DeleteAllByUser_DeletesSubscriptions()
        {
            _context.Subscriptions.AddRange(
                new Subscription { UserId = 50, PackageId = 1 },
                new Subscription { UserId = 50, PackageId = 2 });
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteAllByUser(50);

            Assert.True(result);
            var remaining = await _repository.GetByUserIdAsync(50);
            Assert.Empty(remaining);
        }

        [Fact]
        public async Task DeleteAllByUser_ReturnsFalse_IfNoneFound()
        {
            var result = await _repository.DeleteAllByUser(404);

            Assert.False(result);
        }
    }
}
