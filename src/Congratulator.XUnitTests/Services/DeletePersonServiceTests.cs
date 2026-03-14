using Congratulator.Core.Exceptions;
using Congratulator.Core.Services;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace Congratulator.XUnitTests.Services;

public class DeletePersonServiceTests
{
    private readonly IPersonRepository _personRepository = Substitute.For<IPersonRepository>();
    private readonly DeletePersonService _service;

    public DeletePersonServiceTests()
    {
        _service = new DeletePersonService(_personRepository);
    }

    [Fact]
    public async Task RunAsync_ExistingPerson_DeletesSuccessfully()
    {
        var person = new Person { Id = Guid.NewGuid(), FirstName = "John" };
        _personRepository.GetPersonByIdAsync(person.Id).Returns(person);

        await _service.RunAsync(person.Id);

        await _personRepository.Received(1).DeletePersonAsync(person);
    }

    [Fact]
    public async Task RunAsync_NonExistingPerson_ThrowsPersonNotFoundException()
    {
        _personRepository.GetPersonByIdAsync(Arg.Any<Guid>()).Returns((Person?)null);

        await Assert.ThrowsAsync<PersonNotFoundException>(
            () => _service.RunAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task RunAsync_CallsGetPersonByIdBeforeDelete()
    {
        var personId = Guid.NewGuid();
        var person = new Person { Id = personId, FirstName = "John" };
        _personRepository.GetPersonByIdAsync(personId).Returns(person);

        await _service.RunAsync(personId);

        Received.InOrder(() =>
        {
            _personRepository.GetPersonByIdAsync(personId);
            _personRepository.DeletePersonAsync(person);
        });
    }
}