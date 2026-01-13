using Congratulator.Infrastructure.Data;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces.Repositories;

namespace Congratulator.Infrastructure.Repositories;

public class PersonRepository(CongratulatorDbContext context) : IPersonRepository
{
    public async Task CreatePersonAsync(Person person)
    {
        context.Persons.Add(person);
        await context.SaveChangesAsync();
    }
}