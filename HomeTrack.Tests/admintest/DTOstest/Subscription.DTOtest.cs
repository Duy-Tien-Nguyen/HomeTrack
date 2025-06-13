using HomeTrack.Api.Request;
using Xunit;
using System;

public class SubscriptionDtoTests
{
    [Fact]
    public void SubscriptionDto_ShouldHoldAllValuesCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var dto = new SubscriptionDto
        {
            Id = 1,
            UserId = 42,
            UserName = "khiem",
            PackageId = 3,
            PackageName = "Pro Plan",
            PackagePrice = 199.99m,
            Status = SubscriptionStatus.Active,
            StartsAt = now,
            EndsAt = now.AddDays(30),
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal(42, dto.UserId);
        Assert.Equal("khiem", dto.UserName);
        Assert.Equal(3, dto.PackageId);
        Assert.Equal("Pro Plan", dto.PackageName);
        Assert.Equal(199.99m, dto.PackagePrice);
        Assert.Equal(SubscriptionStatus.Active, dto.Status);
        Assert.Equal("Active", dto.StatusText);
        Assert.Equal(now, dto.StartsAt);
        Assert.Equal(now.AddDays(30), dto.EndsAt);
        Assert.Equal(now, dto.CreatedAt);
        Assert.Equal(now, dto.UpdatedAt);
    }

    [Fact]
    public void CreateSubscriptionDto_ShouldSetValuesCorrectly()
    {
        // Arrange
        var dto = new CreateSubscriptionDto
        {
            UserId = 12,
            PackageId = 5
        };

        // Assert
        Assert.Equal(12, dto.UserId);
        Assert.Equal(5, dto.PackageId);
    }

    [Fact]
    public void UpdateSubscriptionStatusDto_ShouldHoldStatus()
    {
        // Arrange
        var dto = new UpdateSubscriptionStatusDto
        {
            Status = SubscriptionStatus.Expired
        };

        // Assert
        Assert.Equal(SubscriptionStatus.Expired, dto.Status);
    }

    [Fact]
    public void GetSubscriptionByIdReq_ShouldHoldId()
    {
        // Arrange
        var dto = new GetSubscriptionByIdReq
        {
            subcriptionId = 1001
        };

        // Assert
        Assert.Equal(1001, dto.subcriptionId);
    }

    [Fact]
    public void ExpireSubscriptionReq_ShouldHoldAllValues()
    {
        // Arrange
        var dto = new ExpireSubscriptionReq
        {
            subscriptionId = 2002,
            userId = 77
        };

        // Assert
        Assert.Equal(2002, dto.subscriptionId);
        Assert.Equal(77, dto.userId);
    }
}
