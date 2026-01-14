using Congratulator.Core.Exceptions;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.SharedKernel.Interfaces.Repositories;

namespace Congratulator.Core.Services;

public class UpdatePersonService(IPersonRepository personRepository)
{
    public async Task<CreatePersonResponse> RunAsync(Guid personId, UpdatePersonRequest request)
    {
        var person = await personRepository.GetPersonByIdAsync(personId)
                     ?? throw new PersonNotFoundException("Person not found");

        bool hasChanges = false;

        if (request.FirstName is { } fn && fn != person.FirstName) 
            (person.FirstName, hasChanges) = (fn, true);

        if (request.LastName is { } ln && ln != person.LastName) 
            (person.LastName, hasChanges) = (ln, true);

        if (request.BirthDate is { } bd && bd != person.BirthDate) 
            (person.BirthDate, hasChanges) = (bd, true);

        if (request.RelationshipType is { } rt && rt != person.RelationshipType) 
            (person.RelationshipType, hasChanges) = (rt, true);

        if (hasChanges)
        {
            await personRepository.UpdatePersonAsync(person);
        }

        return new CreatePersonResponse
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            BirthDate = person.BirthDate,
            RelationshipType = person.RelationshipType
        };
    }
}