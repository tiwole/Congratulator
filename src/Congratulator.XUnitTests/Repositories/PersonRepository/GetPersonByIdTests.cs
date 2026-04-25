using Congratulator.Infrastructure.Data;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using PersonRepo = global::Congratulator.Infrastructure.Repositories.PersonRepository;

namespace Congratulator.XUnitTests.Repositories.PersonRepository;

public class GetPersonByIdTests : IDisposable
{
    private readonly CongratulatorDbContext _context;
    private readonly PersonRepo _repository;

    public GetPersonByIdTests()
    {
        var options = new DbContextOptionsBuilder<CongratulatorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new CongratulatorDbContext(options);
        var mapper = Substitute.For<IMapper>();
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        var logger = Substitute.For<ILogger<PersonRepo>>();
        _repository = new PersonRepo(_context, mapper, dateTimeProvider);
    }

    [Fact]
    public async Task GetPersonByIdAsync_ExistingId_ReturnsPerson()
    {
        var person = new Person { FirstName = "John", BirthDate = new DateOnly(1990, 1, 1) };
        _context.Persons.Add(person);
        await _context.SaveChangesAsync();

        var result = await _repository.GetPersonByIdAsync(person.Id);

        Assert.NotNull(result);
        Assert.Equal(person.Id, result.Id);
        Assert.Equal("John", result.FirstName);
    }

    [Fact]
    public async Task GetPersonByIdAsync_NonExistingId_ReturnsNull()
    {
        var result = await _repository.GetPersonByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    public void Dispose() => _context.Dispose();
}
