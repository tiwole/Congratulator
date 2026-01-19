using AutoMapper;
using Congratulator.Infrastructure.Data;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Congratulator.Infrastructure.Repositories;

public class PersonRepository(CongratulatorDbContext context, IMapper mapper) : IPersonRepository
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
        var upcomingDays = request.Upcoming ?? 3;
        var startMmdd = today.Month * 100 + today.Day;
        var endMmdd = today.AddDays(upcomingDays).Month * 100 + today.AddDays(upcomingDays).Day;
        
        var query = context.Persons.AsNoTracking();

        // Filter by status.
        if (request.Status.HasValue)
        {
            query = query.Where(p => p.RelationshipType == request.Status.Value);
        }

        // Search by first/last name.
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            query = query.Where(p => (EF.Functions.Like(p.FirstName.ToLower(), $"%{searchTerm}%")) ||
                                     (!string.IsNullOrEmpty(p.LastName) && EF.Functions.Like(p.LastName.ToLower(), $"%{searchTerm}%")));
            
        }
        
        // Filter by upcoming days.
        if (string.IsNullOrWhiteSpace(request.Search) && request.All != true)
        {
            if (endMmdd >= startMmdd)
            {
                query = query.Where(p =>
                    p.BirthDate.Month * 100 + p.BirthDate.Day >= startMmdd &&
                    p.BirthDate.Month * 100 + p.BirthDate.Day <= endMmdd);
            }
            else
            {
                // New year case.
                query = query.Where(p =>
                    p.BirthDate.Month * 100 + p.BirthDate.Day >= startMmdd ||
                    p.BirthDate.Month * 100 + p.BirthDate.Day <= endMmdd);
            }
        }

        // Sort birthdays upcoming first MMDD method.
        var persons = await query
            .OrderBy(x =>
                x.BirthDate.Month * 100 + x.BirthDate.Day >= startMmdd
                    ? x.BirthDate.Month * 100 + x.BirthDate.Day
                    : x.BirthDate.Month * 100 + x.BirthDate.Day + 1200)
            .ThenBy(p => p.BirthDate.Year)
            .ToListAsync();

        return new GetPersonsResponse
        {
            TodayBirthdays = mapper.Map<List<PersonModel>>(persons.Where(x => x.BirthDate.Day == today.Day && x.BirthDate.Month == today.Month)),
            UpcomingBirthdays = mapper.Map<List<PersonModel>>(persons.Where(x => !(x.BirthDate.Day == today.Day && x.BirthDate.Month == today.Month)))
        };
    }
}