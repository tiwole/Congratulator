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

public class DeletePersonTests : IDisposable
{
    private readonly CongratulatorDbContext _context;
    private readonly PersonRepo _repository;

    public DeletePersonTests()
    {
        var options = new DbContextOptionsBuilder<CongratulatorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new CongratulatorDbContext(options);
        var mapper = Substitute.For<IMapper>();
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        var logger = Substitute.For<ILogger<PersonRepo>>();
        _repository = new PersonRepo(_context, mapper, dateTimeProvider, logger);
    }

    [Fact]
    public async Task DeletePersonAsync_RemovesPersonFromDatabase()
    {
        var person = new Person { FirstName = "John", BirthDate = new DateOnly(1990, 1, 1) };
        _context.Persons.Add(person);
        await _context.SaveChangesAsync();

        await _repository.DeletePersonAsync(person);

        var deleted = await _context.Persons.FirstOrDefaultAsync(p => p.Id == person.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeletePersonAsync_DoesNotAffectOtherPersons()
    {
        var person1 = new Person { FirstName = "John", BirthDate = new DateOnly(1990, 1, 1) };
        var person2 = new Person { FirstName = "Jane", BirthDate = new DateOnly(1995, 5, 15) };
        _context.Persons.AddRange(person1, person2);
        await _context.SaveChangesAsync();

        await _repository.DeletePersonAsync(person1);

        Assert.Equal(1, await _context.Persons.CountAsync());
        Assert.NotNull(await _context.Persons.FirstOrDefaultAsync(p => p.Id == person2.Id));
    }

    public void Dispose() => _context.Dispose();
}
