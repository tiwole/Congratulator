using Congratulator.Infrastructure.Data;
using Congratulator.SharedKernel.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Congratulator.XUnitTests.Repositories.PersonRepository;

public class CreatePersonTests
{
    private readonly Infrastructure.Repositories.PersonRepository _repository;
    private readonly List<Person> _testPersons;

    public CreatePersonTests()
    {
        _testPersons = TestHelpers.TestData.GetTestPersons();
        
        var options = new DbContextOptionsBuilder<CongratulatorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CongratulatorDbContext(options);
        //_repository = new Infrastructure.Repositories.PersonRepository(context);
    }

    [Fact]
    public async Task CreatePersonAsync_ShouldAddPerson()
    {
        await _repository.CreatePersonAsync(_testPersons[0]);
    }
}