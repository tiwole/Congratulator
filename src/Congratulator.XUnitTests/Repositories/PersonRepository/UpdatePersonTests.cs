using Congratulator.Infrastructure.Data;
using Congratulator.SharedKernel.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Congratulator.XUnitTests.Repositories.PersonRepository;

public class UpdatePersonTests
{
    private readonly Infrastructure.Repositories.PersonRepository _repository;
    private readonly List<Person> _testPersons;

    public UpdatePersonTests()
    {
        _testPersons = TestHelpers.TestData.GetTestPersons();
        
        var options = new DbContextOptionsBuilder<CongratulatorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CongratulatorDbContext(options);
        _repository = new Infrastructure.Repositories.PersonRepository(context);
    }

    [Fact]
    public async Task UpdatePersonAsync_ShouldUpdatePerson()
    {
        await _repository.CreatePersonAsync(_testPersons[0]);
        
        // Arrange
        var personToUpdate = _testPersons[0];
        personToUpdate.FirstName = "Updated";
        
        // Act
        await _repository.UpdatePersonAsync(personToUpdate);
    }
}
