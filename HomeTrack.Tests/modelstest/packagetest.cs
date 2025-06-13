using System;
using Xunit;
using HomeTrack.Domain;
using System.Collections.Generic;

public class PackageTests
{
    [Fact]
    public void Package_Constructor_ShouldInitializeEmptySubscriptions()
    {
        var package = new Package();

        Assert.NotNull(package.Subscriptions);
        Assert.Empty(package.Subscriptions);
    }

    [Fact]
    public void Package_SetProperties_ShouldStoreCorrectValues()
    {
        var now = DateTime.UtcNow;

        var package = new Package
        {
            Id = 1,
            Name = "Premium Plan",
            Description = "Full access to features",
            Price = 99.99m,
            DurationDays = 30,
            isActive = true,
            CreateAt = now,
            UpdateAt = now
        };

        Assert.Equal(1, package.Id);
        Assert.Equal("Premium Plan", package.Name);
        Assert.Equal("Full access to features", package.Description);
        Assert.Equal(99.99m, package.Price);
        Assert.Equal(30, package.DurationDays);
        Assert.True(package.isActive);
        Assert.Equal(now, package.CreateAt);
        Assert.Equal(now, package.UpdateAt);
    }

    [Fact]
    public void Package_CanAddSubscription_ToCollection()
    {
        var package = new Package { Name = "Basic Plan" };
        var subscription = new Subscription { Id = 101 };

        package.Subscriptions.Add(subscription);

        Assert.Single(package.Subscriptions);
        Assert.Contains(subscription, package.Subscriptions);
    }

    [Fact]
    public void Package_Description_CanBeNull()
    {
        var package = new Package
        {
            Name = "No Description Plan",
            Description = null
        };

        Assert.Null(package.Description);
    }
}
