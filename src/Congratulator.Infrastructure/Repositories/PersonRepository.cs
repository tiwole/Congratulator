using AutoMapper;
using Congratulator.Infrastructure.Data;
using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Interfaces;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.SharedKernel.Contracts.Models.Results;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Congratulator.Infrastructure.Repositories;

public class PersonRepository(CongratulatorDbContext context, IMapper mapper, IDateTimeProvider dateTimeProvider, ILogger<PersonRepository> logger) : IPersonRepository
{
    public async Task CreatePersonAsync(Person person)
    {
        context.Persons.Add(person);
        await context.SaveChangesAsync();
        logger.LogInformation("Person {PersonId} created: {FirstName}", person.Id, person.FirstName);
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
        logger.LogInformation("Person {PersonId} updated", person.Id);
    }

    public async Task DeletePersonAsync(Person person)
    {
        context.Persons.Remove(person);
        await context.SaveChangesAsync();
        logger.LogInformation("Person {PersonId} deleted", person.Id);
    }
    
    public async Task<GetPersonsResponse> GetPersonsAsync(GetPersonsRequest request)
    {
        var today = dateTimeProvider.Today;
        var upcomingDays = request.Upcoming ?? 3;
        var startMmdd = today.Month * 100 + today.Day;
        var endMmdd = today.AddDays(upcomingDays).Month * 100 + today.AddDays(upcomingDays).Day;
        
        var query = context.Persons.AsNoTracking();

        query = ApplyFilters(query, request, startMmdd, endMmdd);

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

    public async Task<GetPagedPersonsResults> GetPagedPersonsAsync(GetPersonsRequest request)
    {
        var query = context.Persons.AsNoTracking();
        
        query = ApplyFilters(query, request);
        
        var totalCount = await query.CountAsync();

        var persons = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize + 1)
            .ToListAsync();

        bool hasNext = persons.Count > request.PageSize;

        if (hasNext)
        {
            persons.RemoveAt(persons.Count - 1);
        }

        return new GetPagedPersonsResults
        {
            TotalCount = totalCount,
            Data = mapper.Map<List<PersonModel>>(persons),
            HasNext = hasNext
        };
    }
    
    private IQueryable<Person> ApplyFilters(IQueryable<Person> query, GetPersonsRequest request, int startMmdd = 0, int endMmdd = 0)
    {
        // Filter by multiple statuses
        if (request.RelationshipTypes != null && request.RelationshipTypes.Count != 0)
        {
            query = query.Where(p => request.RelationshipTypes.Contains(p.RelationshipType));
        }

        // Search by first/last name
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = $"%{request.Search.Trim()}%";

            query = query.Where(p =>
                EF.Functions.ILike(p.FirstName, term) ||
                (!string.IsNullOrEmpty(p.LastName) && EF.Functions.ILike(p.LastName!, term)));
        }

        // Filter by upcoming days
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
                // New year case
                query = query.Where(p =>
                    p.BirthDate.Month * 100 + p.BirthDate.Day >= startMmdd ||
                    p.BirthDate.Month * 100 + p.BirthDate.Day <= endMmdd);
            }
        }
        
        // Apply sorting with optional descending
        if (request.Sort.HasValue)
        {
            bool desc = request.SortDesc == true;

            query = (request.Sort.Value, desc) switch
            {
                (SortVariants.Name, false) => query.OrderBy(p => p.FirstName).ThenBy(p => p.LastName),
                (SortVariants.Name, true)  => query.OrderByDescending(p => p.FirstName).ThenByDescending(p => p.LastName),

                (SortVariants.Age, false) => query.OrderByDescending(p => p.BirthDate), // Oldest first
                (SortVariants.Age, true)  => query.OrderBy(p => p.BirthDate),             // Youngest first

                (SortVariants.Birthday, false) => query.OrderBy(p => p.BirthDate.Month).ThenBy(p => p.BirthDate.Day),
                (SortVariants.Birthday, true)  => query.OrderByDescending(p => p.BirthDate.Month).ThenByDescending(p => p.BirthDate.Day),

                _ => query
            };
        }

        return query;
    }
}