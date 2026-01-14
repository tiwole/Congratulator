using Congratulator.Infrastructure.Data;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Models.Requests;
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
    
    public async Task DeletePersonAsync(Person person)
    {
        context.Persons.Remove(person);
        await context.SaveChangesAsync();
    }
    
    public async Task<GetPersonsResults> GetPersonsAsync(GetPersonsRequest request)
    {
        var query = context.Persons
            .AsNoTracking()
            .Where(x => x.BirthDate < request.Cursor);
        
        // Sorting.
        var persons = await query
            .OrderByDescending(x => x.BirthDate)
            .ThenBy(x => x.Id)
            .Take(request.PageSize + 1)
            .ToListAsync();
        
        // Paging.
        bool hasMore = persons.Count > request.PageSize;

        if (hasMore)
        {
            persons.RemoveAt(persons.Count - 1);
        }
        
        return new GetPersonsResults
        {
            Data = persons,
            HasMore = hasMore
        };
    }
}