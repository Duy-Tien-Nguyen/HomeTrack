using HomeTrack.Api.Request;
using Xunit;
using System;

public class PackageDtoTests
{
    [Fact]
    public void CreatePackageDto_ShouldInitializeWithDefaults()
    {
        // Arrange
        var dto = new CreatePackageDto
        {
            Name = "Premium Plan",
            Price = 99.99m,
            DurationDays = 30
        };

        // Assert
        Assert.Equal("Premium Plan", dto.Name);
        Assert.Equal(99.99m, dto.Price);
        Assert.Equal(30, dto.DurationDays);
        Assert.True(dto.IsActive); // mặc định true
        Assert.Null(dto.Description);
    }

    [Fact]
    public void UpdatePackageDto_ShouldHoldProvidedValues()
    {
        // Arrange
        var dto = new UpdatePackageDto
        {
            Name = "Updated Plan",
            Description = "Extended time",
            Price = 49.99m,
            DurationDays = 15,
            IsActive = false
        };

        // Assert
        Assert.Equal("Updated Plan", dto.Name);
        Assert.Equal("Extended time", dto.Description);
        Assert.Equal(49.99m, dto.Price);
        Assert.Equal(15, dto.DurationDays);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void PackageDto_ShouldBeValidWhenInitialized()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var dto = new PackageDto
        {
            Id = 1,
            Name = "Basic Plan",
            Description = "Basic monthly plan",
            Price = 19.99m,
            DurationDays = 30,
            IsActive = true,
            CreateAt = now,
            UpdateAt = now
        };

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("Basic Plan", dto.Name);
        Assert.Equal("Basic monthly plan", dto.Description);
        Assert.Equal(19.99m, dto.Price);
        Assert.Equal(30, dto.DurationDays);
        Assert.True(dto.IsActive);
        Assert.Equal(now, dto.CreateAt);
        Assert.Equal(now, dto.UpdateAt);
    }

    [Fact]
    public void DeletePackageDto_ShouldHoldPackageId()
    {
        // Arrange
        var dto = new DeletePackageDto
        {
            packageId = "pkg_001"
        };

        // Assert
        Assert.Equal("pkg_001", dto.packageId);
    }
}
