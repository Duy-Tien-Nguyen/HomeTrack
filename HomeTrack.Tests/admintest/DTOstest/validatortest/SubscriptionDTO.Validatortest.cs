using Xunit;
using FluentValidation.TestHelper;
using HomeTrack.Api.Request;
using System;

public class SubscriptionDtoValidatorTests
{
    private readonly SubscriptionDtoValidator _subValidator = new();
    private readonly CreateSubscriptionDtoValidator _createValidator = new();
    private readonly UpdateSubscriptionStatusDtoValidator _statusValidator = new();
    private readonly GetSubscriptionByIdReqValidator _getByIdValidator = new();
    private readonly ExpireSubscriptionReqValidator _expireValidator = new();

    [Fact]
    public void SubscriptionDto_Valid_ShouldPass()
    {
        var dto = new SubscriptionDto
        {
            Id = 1,
            UserId = 1,
            PackageId = 2,
            UserName = "Khiem",
            PackageName = "Premium",
            PackagePrice = 100,
            Status = SubscriptionStatus.Active,
            StartsAt = DateTime.UtcNow,
            EndsAt = DateTime.UtcNow.AddDays(10),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };

        var result = _subValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void SubscriptionDto_InvalidDates_ShouldFail()
    {
        var dto = new SubscriptionDto
        {
            Id = 1,
            UserId = 1,
            PackageId = 1,
            UserName = "Khiem",
            PackageName = "Basic",
            PackagePrice = 10,
            Status = SubscriptionStatus.Active,
            StartsAt = DateTime.UtcNow.AddDays(10),
            EndsAt = DateTime.UtcNow.AddDays(5),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = _subValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.StartsAt);
    }

    [Fact]
    public void SubscriptionDto_EndDateInPast_ShouldFail()
    {
        var dto = new SubscriptionDto
        {
            Id = 1,
            UserId = 1,
            PackageId = 1,
            UserName = "A",
            PackageName = "X",
            PackagePrice = 5,
            Status = SubscriptionStatus.Active,
            StartsAt = DateTime.UtcNow.AddDays(-5),
            EndsAt = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = _subValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.EndsAt);
    }

    [Fact]
    public void CreateSubscriptionDto_Invalid_ShouldFail()
    {
        var dto = new CreateSubscriptionDto
        {
            UserId = 0,
            PackageId = -1
        };

        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
        result.ShouldHaveValidationErrorFor(x => x.PackageId);
    }

    [Fact]
    public void CreateSubscriptionDto_Valid_ShouldPass()
    {
        var dto = new CreateSubscriptionDto
        {
            UserId = 1,
            PackageId = 2
        };

        var result = _createValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateSubscriptionStatusDto_InvalidStatus_ShouldFail()
    {
        var dto = new UpdateSubscriptionStatusDto
        {
            Status = (SubscriptionStatus)999
        };

        var result = _statusValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void GetSubscriptionByIdReq_InvalidId_ShouldFail()
    {
        var dto = new GetSubscriptionByIdReq
        {
            subcriptionId = 0
        };

        var result = _getByIdValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.subcriptionId);
    }

    [Fact]
    public void ExpireSubscriptionReq_InvalidData_ShouldFail()
    {
        var dto = new ExpireSubscriptionReq
        {
            subscriptionId = -1,
            userId = 0
        };

        var result = _expireValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.subscriptionId);
        result.ShouldHaveValidationErrorFor(x => x.userId);
    }

    [Fact]
    public void ExpireSubscriptionReq_Valid_ShouldPass()
    {
        var dto = new ExpireSubscriptionReq
        {
            subscriptionId = 10,
            userId = 1
        };

        var result = _expireValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
