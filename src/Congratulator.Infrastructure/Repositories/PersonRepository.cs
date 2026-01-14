using Congratulator.Infrastructure.Data;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Congratulator.Infrastructure.Repositories;

public class PersonRepository(CongratulatorDbContext context) : IPersonRepository
{
    public async Task CreatePersonAsync(Person person)
    {
        context.Persons.Add(person);
        await context.SaveChangesAsync();
    }
    
    public async Task<Person?> GetPersonByIdAsync(Guid id)
    {
        return await context.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task UpdatePersonAsync(Person person)
    {
        context.Persons.Update(person);
        await context.SaveChangesAsync();
    }
}