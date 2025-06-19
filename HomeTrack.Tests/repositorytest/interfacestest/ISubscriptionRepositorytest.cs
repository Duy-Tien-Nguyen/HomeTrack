using System.Collections.Generic;
using System.Threading.Tasks;
using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using Moq;
using Xunit;

namespace HomeTrack.Tests.Repositories
{
    public class ISubscriptionRepositoryTests
    {
        private readonly Mock<ISubscriptionRepository> _mockRepo;
        private readonly Subscription _sampleSubscription;

        public ISubscriptionRepositoryTests()
        {
            _mockRepo = new Mock<ISubscriptionRepository>();
            _sampleSubscription = new Subscription { Id = 1, UserId = 100, PackageId = 200 };
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsSubscription()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(_sampleSubscription);

            var result = await _mockRepo.Object.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(100, result?.UserId);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllSubscriptions()
        {
            var list = new List<Subscription> { _sampleSubscription };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            var result = await _mockRepo.Object.GetAllAsync();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsUserSubscriptions()
        {
            var list = new List<Subscription> { _sampleSubscription };
            _mockRepo.Setup(r => r.GetByUserIdAsync(100)).ReturnsAsync(list);

            var result = await _mockRepo.Object.GetByUserIdAsync(100);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(100, result[0].UserId);
        }

        [Fact]
        public async Task GetActiveSubscriptionByUserIdAsync_ReturnsActiveSubscription()
        {
            _mockRepo.Setup(r => r.GetActiveSubscriptionByUserIdAsync(100, 200))
                     .ReturnsAsync(_sampleSubscription);

            var result = await _mockRepo.Object.GetActiveSubscriptionByUserIdAsync(100, 200);

            Assert.NotNull(result);
            Assert.Equal(200, result?.PackageId);
        }

        [Fact]
        public async Task AddAsync_ReturnsAddedSubscription()
        {
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Subscription>())).ReturnsAsync(_sampleSubscription);

            var result = await _mockRepo.Object.AddAsync(new Subscription());

            Assert.NotNull(result);
            Assert.Equal(_sampleSubscription.Id, result.Id);
        }

        [Fact]
        public async Task UpdateAsync_DoesNotThrow()
        {
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Subscription>())).Returns(Task.CompletedTask);

            var exception = await Record.ExceptionAsync(() => _mockRepo.Object.UpdateAsync(_sampleSubscription));

            Assert.Null(exception);
        }

        [Fact]
        public async Task DeleteAllByUser_ReturnsTrue()
        {
            _mockRepo.Setup(r => r.DeleteAllByUser(100)).ReturnsAsync(true);

            var result = await _mockRepo.Object.DeleteAllByUser(100);

            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue()
        {
            _mockRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

            var result = await _mockRepo.Object.ExistsAsync(1);

            Assert.True(result);
        }
    }
}
