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

public class UpdatePersonTests : IDisposable
{
    private readonly CongratulatorDbContext _context;
    private readonly PersonRepo _repository;

    public UpdatePersonTests()
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
    public async Task UpdatePersonAsync_UpdatesFieldsInDatabase()
    {
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = new DateOnly(1990, 1, 1),
            RelationshipType = RelationshipType.Friend
        };
        _context.Persons.Add(person);
        await _context.SaveChangesAsync();
        _context.Entry(person).State = EntityState.Detached;

        person.FirstName = "Jane";
        person.LastName = "Smith";
        await _repository.UpdatePersonAsync(person);

        var updated = await _context.Persons.AsNoTracking().FirstAsync(p => p.Id == person.Id);
        Assert.Equal("Jane", updated.FirstName);
        Assert.Equal("Smith", updated.LastName);
    }

    public void Dispose() => _context.Dispose();
}
