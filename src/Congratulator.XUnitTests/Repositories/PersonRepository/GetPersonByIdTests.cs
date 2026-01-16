using Congratulator.Infrastructure.Data;
using Congratulator.SharedKernel.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Congratulator.XUnitTests.Repositories.PersonRepository;

public class GetPersonByIdTests
{
    private readonly Infrastructure.Repositories.PersonRepository _repository;
    private readonly List<Person> _testPersons;

    public GetPersonByIdTests()
    {
        _testPersons = TestHelpers.TestData.GetTestPersons();
        
        var options = new DbContextOptionsBuilder<CongratulatorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CongratulatorDbContext(options);
        _repository = new Infrastructure.Repositories.PersonRepository(context);
    }

    [Fact]
    public async Task GetPersonByIdAsync_ExistingId_ShouldReturnPerson()
    {
        // Arrange
        var expectedPerson = _testPersons[0];
        await _repository.CreatePersonAsync(expectedPerson);
        
        // Act
        var result = await _repository.GetPersonByIdAsync(expectedPerson.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPerson.Id, result.Id);
        Assert.Equal(expectedPerson.FirstName, result.FirstName);
    }

    [Fact]
    public async Task GetPersonByIdAsync_NonExistingId_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        
        // Act
        var result = await _repository.GetPersonByIdAsync(nonExistingId);
        
        // Assert
        Assert.Null(result);
    }
}
