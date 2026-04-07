using Congratulator.Core.Validators;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Interfaces;
using NSubstitute;
using Xunit;

namespace Congratulator.XUnitTests.Validators;

public class UpdatePersonValidatorTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly UpdatePersonValidator _validator;

    public UpdatePersonValidatorTests()
    {
        _dateTimeProvider.Today.Returns(new DateOnly(2026, 3, 14));
        _dateTimeProvider.UtcNow.Returns(new DateTime(2026, 3, 14, 12, 0, 0, DateTimeKind.Utc));
        _validator = new UpdatePersonValidator(_dateTimeProvider);
    }

    [Fact]
    public void Validate_ValidRequest_IsValid()
    {
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            BirthDate = new DateOnly(1990, 1, 1)
        };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_FirstNameOver32Chars_IsInvalid()
    {
        var request = new CreatePersonRequest
        {
            FirstName = new string('A', 33),
            BirthDate = new DateOnly(1990, 1, 1)
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
    }

    [Fact]
    public void Validate_EmptyFirstName_IsValid()
    {
        // UpdatePersonValidator does NOT require FirstName (unlike CreatePersonValidator)
        var request = new CreatePersonRequest
        {
            FirstName = "",
            BirthDate = new DateOnly(1990, 1, 1)
        };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_LastNameOver64Chars_IsInvalid()
    {
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            LastName = new string('A', 65),
            BirthDate = new DateOnly(1990, 1, 1)
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LastName");
    }

    [Fact]
    public void Validate_BirthDateToday_IsValid()
    {
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            BirthDate = new DateOnly(2026, 3, 14) // today - LessThanOrEqualTo
        };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_BirthDateTomorrow_IsInvalid()
    {
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            BirthDate = new DateOnly(2026, 3, 15)
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "BirthDate");
    }

    [Fact]
    public void Validate_BirthDateOver120YearsAgo_IsInvalid()
    {
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            BirthDate = new DateOnly(1906, 3, 13)
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "BirthDate");
    }
}
