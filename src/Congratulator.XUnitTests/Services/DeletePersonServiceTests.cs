using Congratulator.Core.Exceptions;
using Congratulator.Core.Services;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces.Repositories;
using Congratulator.SharedKernel.Interfaces.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Congratulator.XUnitTests.Services;

public class DeletePersonServiceTests
{
    private readonly IPersonRepository _personRepository = Substitute.For<IPersonRepository>();
    private readonly IStorageService _storageService = Substitute.For<IStorageService>();
    private readonly ILogger<CreatePersonService> _logger = Substitute.For<ILogger<CreatePersonService>>();
    private readonly DeletePersonService _service;

    public DeletePersonServiceTests()
    {
        _service = new DeletePersonService(_personRepository, _storageService, _logger);
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