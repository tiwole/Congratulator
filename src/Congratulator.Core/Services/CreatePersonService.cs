using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces.Repositories;

namespace Congratulator.Core.Services;

public class CreatePersonService(IPersonRepository personRepository)
{
    public async Task<CreatePersonResponse> RunAsync(CreatePersonRequest request)
    {
        var person = new Person
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            RelationshipType = request.RelationshipType ?? RelationshipType.Unknown
        };

        await personRepository.CreatePersonAsync(person);

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