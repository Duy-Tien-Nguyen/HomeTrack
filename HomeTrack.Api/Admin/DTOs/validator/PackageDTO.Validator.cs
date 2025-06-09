using HomeTrack.Api.Request;
using FluentValidation;

public class PackageDtoValidator : AbstractValidator<PackageDto>
{
  public PackageDtoValidator()
  {
    RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Package name is required")
        .MaximumLength(100).WithMessage("Package name cannot exceed 100 characters");

    RuleFor(x => x.Price)
        .GreaterThan(0).WithMessage("Package price must be greater than zero");

    RuleFor(x => x.Description)
        .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
  }
}

public class CreatePackageDtoValidator : AbstractValidator<CreatePackageDto>
{
  public CreatePackageDtoValidator()
  {
    RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Package name is required")
        .MaximumLength(100).WithMessage("Package name cannot exceed 100 characters");

    RuleFor(x => x.Price)
        .GreaterThan(0).WithMessage("Package price must be greater than zero");

    RuleFor(x => x.Description)
        .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

    RuleFor(x => x.DurationDays)
        .GreaterThan(0).WithMessage("Package duration must be greater than zero");
  }
}

public class UpdatePackageDtoValidator : AbstractValidator<UpdatePackageDto>
{
  public UpdatePackageDtoValidator()
  {
    RuleFor(x => x.IsActive)
        .NotNull().WithMessage("IsActive must be defined");

    RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Package name is required")
        .MaximumLength(100).WithMessage("Package name cannot exceed 100 characters");

    RuleFor(x => x.Price)
        .GreaterThan(0).WithMessage("Package price must be greater than zero");

    RuleFor(x => x.Description)
        .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

    RuleFor(x => x.DurationDays)
        .GreaterThan(0).WithMessage("Package duration must be greater than zero");
  }
}

public class DeletePackageDtoValidator : AbstractValidator<DeletePackageDto>
{
  public DeletePackageDtoValidator()
  {
    RuleFor(x => x.packageId)
        .NotEmpty().WithMessage("Package ID is required")
        .Matches(@"^\d+$").WithMessage("Package ID must be a valid number");
  }
}
