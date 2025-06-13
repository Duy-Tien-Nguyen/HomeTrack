using System;
using Xunit;
using HomeTrack.Domain;

public class SubscriptionTests
{
    [Fact]
    public void Subscription_DefaultInitialization_ShouldSetDefaultValues()
    {
        var subscription = new Subscription();

        Assert.Equal(0, subscription.Id);
        Assert.Equal(0, subscription.UserId);
        Assert.Equal(0, subscription.PackageId);
        Assert.NotNull(subscription.User);
        Assert.NotNull(subscription.Package);
        Assert.Equal(default, subscription.StartsAt);
        Assert.Equal(default, subscription.EndsAt);
        Assert.Equal(default, subscription.CreatedAt);
        Assert.Equal(default, subscription.UpdatedAt);
        Assert.Equal(SubscriptionStatus.Pending, subscription.Status); // nếu enum mặc định là 0
    }

    [Fact]
    public void Subscription_SetProperties_ShouldStoreCorrectValues()
    {
        var now = DateTime.UtcNow;
        var subscription = new Subscription
        {
            Id = 1,
            UserId = 10,
            PackageId = 100,
            Status = SubscriptionStatus.Active,
            StartsAt = now,
            EndsAt = now.AddDays(30),
            CreatedAt = now,
            UpdatedAt = now
        };

        var user = new User { Id = 10, Email = "user@example.com" };
        var package = new Package { Id = 100, Name = "Premium" };

        subscription.User = user;
        subscription.Package = package;

        Assert.Equal(1, subscription.Id);
        Assert.Equal(10, subscription.UserId);
        Assert.Equal(100, subscription.PackageId);
        Assert.Equal(SubscriptionStatus.Active, subscription.Status);
        Assert.Equal(now, subscription.StartsAt);
        Assert.Equal(now.AddDays(30), subscription.EndsAt);
        Assert.Equal(now, subscription.CreatedAt);
        Assert.Equal(now, subscription.UpdatedAt);
        Assert.Equal(user, subscription.User);
        Assert.Equal(package, subscription.Package);
    }

    [Fact]
    public void Subscription_StartDate_ShouldBeBeforeEndDate()
    {
        var now = DateTime.UtcNow;
        var subscription = new Subscription
        {
            StartsAt = now,
            EndsAt = now.AddDays(5)
        };

        Assert.True(subscription.StartsAt <= subscription.EndsAt);
    }

    [Fact]
    public void Subscription_AllowsFutureEndDate()
    {
        var future = DateTime.UtcNow.AddMonths(1);
        var subscription = new Subscription
        {
            EndsAt = future
        };

        Assert.True(subscription.EndsAt > DateTime.UtcNow);
    }
}
public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
}
