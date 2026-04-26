using Congratulator.Core.Validators;
using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Interfaces;
using NSubstitute;
using Xunit;

namespace Congratulator.XUnitTests.Validators;

public class CreatePersonValidatorTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly CreatePersonValidator _validator;

    public CreatePersonValidatorTests()
    {
        _dateTimeProvider.Today.Returns(new DateOnly(2026, 3, 14));
        _dateTimeProvider.UtcNow.Returns(new DateTime(2026, 3, 14, 12, 0, 0, DateTimeKind.Utc));
        _validator = new CreatePersonValidator(_dateTimeProvider);
    }

    private static CreatePersonRequest ValidRequest() => new()
    {
        FirstName = "John",
        LastName = "Doe",
        BirthDate = new DateOnly(1990, 1, 1),
        RelationshipType = RelationshipType.Friend.ToString()
    };

    [Fact]
    public void Validate_ValidRequest_IsValid()
    {
        var result = _validator.Validate(ValidRequest());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyFirstName_IsInvalid()
    {
        var request = ValidRequest();
        request.FirstName = "";

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
    }

    [Fact]
    public void Validate_FirstNameOver32Chars_IsInvalid()
    {
        var request = ValidRequest();
        request.FirstName = new string('A', 33);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
    }

    [Fact]
    public void Validate_FirstName32Chars_IsValid()
    {
        var request = ValidRequest();
        request.FirstName = new string('A', 32);

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_LastNameOver64Chars_IsInvalid()
    {
        var request = ValidRequest();
        request.LastName = new string('A', 65);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LastName");
    }

    [Fact]
    public void Validate_LastName64Chars_IsValid()
    {
        var request = ValidRequest();
        request.LastName = new string('A', 64);

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_NullLastName_IsValid()
    {
        var request = ValidRequest();
        request.LastName = null;

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_BirthDateTomorrow_IsInvalid()
    {
        var request = ValidRequest();
        request.BirthDate = new DateOnly(2026, 3, 15);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "BirthDate");
    }

    [Fact]
    public void Validate_BirthDateToday_IsValid()
    {
        var request = ValidRequest();
        request.BirthDate = new DateOnly(2026, 3, 14);

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_BirthDateOver120YearsAgo_IsInvalid()
    {
        var request = ValidRequest();
        request.BirthDate = new DateOnly(1906, 3, 13);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "BirthDate");
    }

    [Fact]
    public void Validate_BirthDateFuture_IsInvalid()
    {
        var request = ValidRequest();
        request.BirthDate = new DateOnly(2027, 1, 1);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "BirthDate");
    }

    [Fact]
    public void Validate_DefaultBirthDate_IsInvalid()
    {
        var request = ValidRequest();
        request.BirthDate = default;

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }
}
