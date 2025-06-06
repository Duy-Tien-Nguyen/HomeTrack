using HomeTrack.Api.Request;
using FluentValidation;

public class SubscriptionDtoValidator : AbstractValidator<SubscriptionDto>
{
  public SubscriptionDtoValidator()
  {
    RuleFor(x => x.UserId)
        .GreaterThan(0).WithMessage("User ID must be greater than zero");

    RuleFor(x => x.PackageId)
        .GreaterThan(0).WithMessage("Package ID must be greater than zero");

    RuleFor(x => x.UserName)
        .NotEmpty().WithMessage("User name is required")
        .MaximumLength(100).WithMessage("User name cannot exceed 100 characters");

    RuleFor(x => x.PackageName)
        .NotEmpty().WithMessage("Package name is required")
        .MaximumLength(100).WithMessage("Package name cannot exceed 100 characters");

    RuleFor(x => x.PackagePrice)
        .GreaterThan(0).WithMessage("Package price must be greater than zero");

    RuleFor(x => x.StartsAt)
        .LessThanOrEqualTo(x => x.EndsAt).WithMessage("Start date must be before or equal to end date");

    RuleFor(x => x.EndsAt)
        .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("End date must be in the future");
  }
}

public class CreateSubscriptionDtoValidator : AbstractValidator<CreateSubscriptionDto>
{
  public CreateSubscriptionDtoValidator()
  {
    RuleFor(x => x.UserId)
        .GreaterThan(0).WithMessage("User ID must be greater than zero");

    RuleFor(x => x.PackageId)
        .GreaterThan(0).WithMessage("Package ID must be greater than zero");
  }
}

public class UpdateSubscriptionStatusDtoValidator : AbstractValidator<UpdateSubscriptionStatusDto>
{
  public UpdateSubscriptionStatusDtoValidator()
  {
    RuleFor(x => x.Status)
        .IsInEnum().WithMessage("Invalid subscription status");
  }
}
public class GetSubscriptionByIdReqValidator : AbstractValidator<GetSubscriptionByIdReq>
{
  public GetSubscriptionByIdReqValidator()
  {
    RuleFor(x => x.subcriptionId)
        .GreaterThan(0).WithMessage("Subscription ID must be greater than zero");
  }
}
public class ExpireSubscriptionReqValidator : AbstractValidator<ExpireSubscriptionReq>
{
  public ExpireSubscriptionReqValidator()
  {
    RuleFor(x => x.subscriptionId)
        .GreaterThan(0).WithMessage("Subscription ID must be greater than zero");

    RuleFor(x => x.userId)
        .GreaterThan(0).WithMessage("User ID must be greater than zero");
  }
}

