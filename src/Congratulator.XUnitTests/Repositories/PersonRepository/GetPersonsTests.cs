using Congratulator.Infrastructure.Data;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Congratulator.XUnitTests.Repositories.PersonRepository;

public class GetPersonsTests
{
    private readonly Infrastructure.Repositories.PersonRepository _repository;
    private readonly List<Person> _testPersons;

    public GetPersonsTests()
    {
        _testPersons = TestHelpers.TestData.GetTestPersons();
        
        var options = new DbContextOptionsBuilder<CongratulatorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CongratulatorDbContext(options);
        _repository = new Infrastructure.Repositories.PersonRepository(context);
    }

    [Fact]
    public async Task GetPersonsAsync_WithPaging_ShouldReturnCorrectPage()
    {
        // Arrange
        var request = new GetPersonsRequest
        {
            Cursor = DateOnly.MaxValue,
            PageSize = 2
        };
        
        await _repository.CreatePersonAsync(_testPersons[0]);
        await _repository.CreatePersonAsync(_testPersons[1]);
        await _repository.CreatePersonAsync(_testPersons[2]);
        
        // Act
        var result = await _repository.GetPersonsAsync(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.True(result.HasMore);
    }

    [Fact]
    public async Task GetPersonsAsync_LastPage_ShouldSetHasMoreToFalse()
    {
        // Arrange
        var request = new GetPersonsRequest
        {
            Cursor = DateOnly.MaxValue,
            PageSize = 3
        };

        await _repository.CreatePersonAsync(_testPersons[0]);
        await _repository.CreatePersonAsync(_testPersons[1]);
        await _repository.CreatePersonAsync(_testPersons[2]);
        
        // Act
        var result = await _repository.GetPersonsAsync(request);
        
        // Assert
        Assert.False(result.HasMore);
        Assert.Equal(3, result.Data.Count);
    }

    [Fact]
    public async Task GetPersonsAsync_WithCursor_ShouldReturnOnlyOlderPersons()
    {
        // Arrange
        var cursorDate = new DateOnly(1993, 1, 1);
        var request = new GetPersonsRequest
        {
            Cursor = cursorDate,
            PageSize = 10
        };
        
        await _repository.CreatePersonAsync(_testPersons[0]);
        await _repository.CreatePersonAsync(_testPersons[1]);
        await _repository.CreatePersonAsync(_testPersons[2]);
        
        // Act
        var result = await _repository.GetPersonsAsync(request);
        
        // Assert
        Assert.All(result.Data, p => Assert.True(p.BirthDate < cursorDate));
    }
}
