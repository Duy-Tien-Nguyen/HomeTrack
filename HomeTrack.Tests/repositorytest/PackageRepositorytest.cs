using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeTrack.Domain;
using HomeTrack.Infrastructure.Data;
using HomeTrack.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HomeTrack.Tests.Repositories
{
    public class PackageRepositoryTests
    {
        private readonly ApplicationDBContext _context;
        private readonly PackageRepository _repository;
        private readonly Mock<ILogger<PackageRepository>> _loggerMock;

        public PackageRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;
            _context = new ApplicationDBContext(options);
            _loggerMock = new Mock<ILogger<PackageRepository>>();
            _repository = new PackageRepository(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddAndReturnPackage()
        {
            var package = new Package { Name = "Test", Price = 100, DurationDays = 30 };

            var result = await _repository.AddAsync(package);

            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.NotEqual(default, result.CreateAt);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnPackage_WhenExists()
        {
            var package = new Package { Name = "Sample", Price = 99, DurationDays = 10 };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(package.Id);

            Assert.NotNull(result);
            Assert.Equal("Sample", result!.Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllPackages()
        {
            _context.Packages.Add(new Package { Name = "P1" });
            _context.Packages.Add(new Package { Name = "P2" });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result!.Count);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePackage_WhenExists()
        {
            var package = new Package { Name = "Old", Price = 50 };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            package.Name = "Updated";
            package.Price = 200;
            var result = await _repository.UpadateAsync(package);

            Assert.True(result);
            var updated = await _context.Packages.FindAsync(package.Id);
            Assert.Equal("Updated", updated!.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound()
        {
            var fake = new Package { Id = 999, Name = "None" };

            var result = await _repository.UpadateAsync(fake);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemovePackage_WhenExists()
        {
            var package = new Package { Name = "ToDelete" };
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteAsync(package.Id);

            Assert.True(result);
            var check = await _context.Packages.FindAsync(package.Id);
            Assert.Null(check);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
        {
            var result = await _repository.DeleteAsync(999);

            Assert.False(result);
        }
    }
}
