using Xunit;
using FluentValidation.TestHelper;
using HomeTrack.Api.Request;

public class PackageDtoValidatorTests
{
    private readonly PackageDtoValidator _packageValidator = new();
    private readonly CreatePackageDtoValidator _createValidator = new();
    private readonly UpdatePackageDtoValidator _updateValidator = new();
    private readonly DeletePackageDtoValidator _deleteValidator = new();

    [Fact]
    public void PackageDto_ValidData_ShouldPass()
    {
        var dto = new PackageDto
        {
            Id = 1,
            Name = "Premium",
            Description = "Full access",
            Price = 100,
            DurationDays = 30,
            IsActive = true,
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow
        };

        var result = _packageValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreatePackageDto_InvalidName_ShouldFail()
    {
        var dto = new CreatePackageDto
        {
            Name = "",
            Description = "Trial",
            Price = 20,
            DurationDays = 7
        };

        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CreatePackageDto_NegativePrice_ShouldFail()
    {
        var dto = new CreatePackageDto
        {
            Name = "Basic",
            Description = "Test",
            Price = -5,
            DurationDays = 30
        };

        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void CreatePackageDto_ZeroDuration_ShouldFail()
    {
        var dto = new CreatePackageDto
        {
            Name = "Trial",
            Description = "Test",
            Price = 10,
            DurationDays = 0
        };

        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DurationDays);
    }

    [Fact]
    public void UpdatePackageDto_MissingName_ShouldFail()
    {
        var dto = new UpdatePackageDto
        {
            Name = "",
            Description = "Desc",
            Price = 50,
            DurationDays = 15,
            IsActive = true
        };

        var result = _updateValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void UpdatePackageDto_Valid_ShouldPass()
    {
        var dto = new UpdatePackageDto
        {
            Name = "Updated Name",
            Description = "Updated description",
            Price = 100,
            DurationDays = 60,
            IsActive = true
        };

        var result = _updateValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("12abc")]
    public void DeletePackageDto_InvalidId_ShouldFail(string input)
    {
        var dto = new DeletePackageDto
        {
            packageId = input
        };

        var result = _deleteValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.packageId);
    }

    [Fact]
    public void DeletePackageDto_ValidId_ShouldPass()
    {
        var dto = new DeletePackageDto
        {
            packageId = "123"
        };

        var result = _deleteValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
