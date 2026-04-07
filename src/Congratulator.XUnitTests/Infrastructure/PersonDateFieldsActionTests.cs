using Congratulator.Infrastructure.AutoMapperProfiles;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces;
using NSubstitute;
using Xunit;

namespace Congratulator.XUnitTests.Infrastructure;

public class PersonDateFieldsActionTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly PersonDateFieldsAction _action;

    public PersonDateFieldsActionTests()
    {
        _dateTimeProvider.Today.Returns(new DateOnly(2026, 3, 14));
        _action = new PersonDateFieldsAction(_dateTimeProvider);
    }

    [Fact]
    public void Process_BirthdayToday_Age36_DaysUntil0()
    {
        var person = new Person { FirstName = "John", BirthDate = new DateOnly(1990, 3, 14) };
        var model = new PersonModel();

        _action.Process(person, model, null!);

        Assert.Equal(36, model.Age);
        Assert.Equal(new DateOnly(2026, 3, 14), model.NextBirthday);
        Assert.Equal(0, model.DaysUntilBirthday);
    }

    [Fact]
    public void Process_BirthdayTomorrow_Age35_DaysUntil1()
    {
        var person = new Person { FirstName = "John", BirthDate = new DateOnly(1990, 3, 15) };
        var model = new PersonModel();

        _action.Process(person, model, null!);

        Assert.Equal(35, model.Age);
        Assert.Equal(new DateOnly(2026, 3, 15), model.NextBirthday);
        Assert.Equal(1, model.DaysUntilBirthday);
    }

    [Fact]
    public void Process_BirthdayYesterday_Age36_NextBirthdayNextYear()
    {
        var person = new Person { FirstName = "John", BirthDate = new DateOnly(1990, 3, 13) };
        var model = new PersonModel();

        _action.Process(person, model, null!);

        Assert.Equal(36, model.Age);
        Assert.Equal(new DateOnly(2027, 3, 13), model.NextBirthday);
        Assert.Equal(364, model.DaysUntilBirthday);
    }

    [Fact]
    public void Process_BirthdayJanuary_NextBirthdayNextYear()
    {
        var person = new Person { FirstName = "John", BirthDate = new DateOnly(1990, 1, 1) };
        var model = new PersonModel();

        _action.Process(person, model, null!);

        Assert.Equal(36, model.Age);
        Assert.Equal(new DateOnly(2027, 1, 1), model.NextBirthday);
    }

    [Fact]
    public void Process_BirthdayDecember_NextBirthdayThisYear()
    {
        var person = new Person { FirstName = "John", BirthDate = new DateOnly(1990, 12, 25) };
        var model = new PersonModel();

        _action.Process(person, model, null!);

        Assert.Equal(35, model.Age);
        Assert.Equal(new DateOnly(2026, 12, 25), model.NextBirthday);
    }

    [Fact]
    public void Process_BornThisYear_Age0()
    {
        var person = new Person { FirstName = "Baby", BirthDate = new DateOnly(2026, 1, 1) };
        var model = new PersonModel();

        _action.Process(person, model, null!);

        Assert.Equal(0, model.Age);
    }
}
