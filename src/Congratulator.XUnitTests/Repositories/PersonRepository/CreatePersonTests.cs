using Congratulator.Infrastructure.Data;
using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using PersonRepo = global::Congratulator.Infrastructure.Repositories.PersonRepository;

namespace Congratulator.XUnitTests.Repositories.PersonRepository;

public class CreatePersonTests : IDisposable
{
    private readonly CongratulatorDbContext _context;
    private readonly PersonRepo _repository;

    public CreatePersonTests()
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
    public async Task CreatePersonAsync_AddsPersonToDatabase()
    {
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = new DateOnly(1990, 1, 1),
            RelationshipType = RelationshipType.Friend
        };

        await _repository.CreatePersonAsync(person);

        var saved = await _context.Persons.FirstOrDefaultAsync(p => p.Id == person.Id);
        Assert.NotNull(saved);
        Assert.Equal("John", saved.FirstName);
        Assert.Equal("Doe", saved.LastName);
    }

    [Fact]
    public async Task CreatePersonAsync_GeneratesId()
    {
        var person = new Person { FirstName = "John", BirthDate = new DateOnly(1990, 1, 1) };

        await _repository.CreatePersonAsync(person);

        Assert.NotEqual(Guid.Empty, person.Id);
    }

    public void Dispose() => _context.Dispose();
}
