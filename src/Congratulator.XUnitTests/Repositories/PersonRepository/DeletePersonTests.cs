using Congratulator.Infrastructure.Data;
using Congratulator.SharedKernel.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Congratulator.XUnitTests.Repositories.PersonRepository;

public class DeletePersonTests
{
    private readonly Infrastructure.Repositories.PersonRepository _repository;
    private readonly List<Person> _testPersons;

    public DeletePersonTests()
    {
        _testPersons = TestHelpers.TestData.GetTestPersons();
        
        var options = new DbContextOptionsBuilder<CongratulatorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CongratulatorDbContext(options);
        //_repository = new Infrastructure.Repositories.PersonRepository(context);
    }

    [Fact]
    public async Task DeletePersonAsync_ShouldRemovePerson()
    {
        // Arrange
        var person = _testPersons[0];
        await _repository.CreatePersonAsync(person);

        // Act
        await _repository.DeletePersonAsync(person);

        // Assert
        var result = await _repository.GetPersonByIdAsync(person.Id);
        Assert.Null(result);
    }
}
