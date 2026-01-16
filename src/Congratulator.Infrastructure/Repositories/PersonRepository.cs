using Congratulator.Infrastructure.Data;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Contracts.Models.Responses;
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
    
    public async Task<GetPersonsResponse> GetPersonsAsync(GetPersonsRequest request)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        // Sort birthdays upcoming first MMDD method.
        var query = context.Persons
            .OrderBy(x => 
                x.BirthDate.Month * 100 + x.BirthDate.Day >= today.Month * 100 + today.Day
                    ? x.BirthDate.Month * 100 + x.BirthDate.Day
                    : x.BirthDate.Month * 100 + x.BirthDate.Day + 1200)
            .ThenBy(p => p.BirthDate.Year)
            .AsNoTracking();
        
        var persons = await query
            .Take(request.Size)
            .ToListAsync();

        return new GetPersonsResponse
        {
            TodayBirthdays = persons.Where(x => x.BirthDate.Day == today.Day && x.BirthDate.Month == today.Month).ToList(),
            UpcomingBirthdays = persons.Where(x => x.BirthDate.Day != today.Day || x.BirthDate.Month != today.Month).ToList()
        };
    }
}