using Congratulator.Core.Exceptions;
using Congratulator.Core.Services;
using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace Congratulator.XUnitTests.Services;

public class UpdatePersonServiceTests
{
    private readonly IPersonRepository _personRepository = Substitute.For<IPersonRepository>();
    private readonly UpdatePersonService _service;

    public UpdatePersonServiceTests()
    {
        _service = new UpdatePersonService(_personRepository);
    }

    private static Person CreateTestPerson() => new()
    {
        Id = Guid.NewGuid(),
        FirstName = "John",
        LastName = "Doe",
        BirthDate = new DateOnly(1990, 1, 1),
        RelationshipType = RelationshipType.Friend
    };

    [Fact]
    public async Task RunAsync_PersonNotFound_ThrowsPersonNotFoundException()
    {
        _personRepository.GetPersonByIdAsync(Arg.Any<Guid>()).Returns((Person?)null);

        await Assert.ThrowsAsync<PersonNotFoundException>(
            () => _service.RunAsync(Guid.NewGuid(), new UpdatePersonRequest()));
    }

    [Fact]
    public async Task RunAsync_NoChanges_DoesNotCallUpdate()
    {
        var person = CreateTestPerson();
        _personRepository.GetPersonByIdAsync(person.Id).Returns(person);

        var request = new UpdatePersonRequest
        {
            FirstName = person.FirstName,
            LastName = person.LastName,
            BirthDate = person.BirthDate,
            RelationshipType = person.RelationshipType
        };

        await _service.RunAsync(person.Id, request);

        await _personRepository.DidNotReceive().UpdatePersonAsync(Arg.Any<Person>());
    }

    [Fact]
    public async Task RunAsync_AllNullFields_DoesNotCallUpdate()
    {
        var person = CreateTestPerson();
        _personRepository.GetPersonByIdAsync(person.Id).Returns(person);

        await _service.RunAsync(person.Id, new UpdatePersonRequest());

        await _personRepository.DidNotReceive().UpdatePersonAsync(Arg.Any<Person>());
    }

    [Fact]
    public async Task RunAsync_FirstNameChanged_UpdatesCalled()
    {
        var person = CreateTestPerson();
        _personRepository.GetPersonByIdAsync(person.Id).Returns(person);

        var request = new UpdatePersonRequest { FirstName = "Jane" };

        var result = await _service.RunAsync(person.Id, request);

        Assert.Equal("Jane", result.FirstName);
        await _personRepository.Received(1).UpdatePersonAsync(Arg.Any<Person>());
    }

    [Fact]
    public async Task RunAsync_LastNameChanged_UpdatesCalled()
    {
        var person = CreateTestPerson();
        _personRepository.GetPersonByIdAsync(person.Id).Returns(person);

        var request = new UpdatePersonRequest { LastName = "Smith" };

        var result = await _service.RunAsync(person.Id, request);

        Assert.Equal("Smith", result.LastName);
        await _personRepository.Received(1).UpdatePersonAsync(Arg.Any<Person>());
    }

    [Fact]
    public async Task RunAsync_BirthDateChanged_UpdatesCalled()
    {
        var person = CreateTestPerson();
        _personRepository.GetPersonByIdAsync(person.Id).Returns(person);

        var newDate = new DateOnly(2000, 6, 15);
        var request = new UpdatePersonRequest { BirthDate = newDate };

        var result = await _service.RunAsync(person.Id, request);

        Assert.Equal(newDate, result.BirthDate);
        await _personRepository.Received(1).UpdatePersonAsync(Arg.Any<Person>());
    }

    [Fact]
    public async Task RunAsync_RelationshipTypeChanged_UpdatesCalled()
    {
        var person = CreateTestPerson();
        _personRepository.GetPersonByIdAsync(person.Id).Returns(person);

        var request = new UpdatePersonRequest { RelationshipType = RelationshipType.Family };

        var result = await _service.RunAsync(person.Id, request);

        Assert.Equal(RelationshipType.Family, result.RelationshipType);
        await _personRepository.Received(1).UpdatePersonAsync(Arg.Any<Person>());
    }

    [Fact]
    public async Task RunAsync_ReturnsResponseWithAllFields()
    {
        var person = CreateTestPerson();
        _personRepository.GetPersonByIdAsync(person.Id).Returns(person);

        var result = await _service.RunAsync(person.Id, new UpdatePersonRequest());

        Assert.Equal(person.Id, result.Id);
        Assert.Equal(person.FirstName, result.FirstName);
        Assert.Equal(person.LastName, result.LastName);
        Assert.Equal(person.BirthDate, result.BirthDate);
        Assert.Equal(person.RelationshipType, result.RelationshipType);
    }
}