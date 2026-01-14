using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces.Repositories;

namespace Congratulator.Core.Services;

public class GetPersonsService(IPersonRepository personRepository)
{
    public async Task<PaginatedResponse<Person>> RunAsync(GetPersonsRequest request)
    {
        var dataResult = await personRepository.GetPersonsAsync(request);
        var result = new PaginatedResponse<Person>
        {
            Data = dataResult.Data,
                // TODO: Data = mapper.Map<List<PersonModel>>(dataResult.Data),
            NextCursor = dataResult.Data.Any() ? dataResult.Data.Last().BirthDate : null,
            HasMore = dataResult.HasMore
        };
        return result;
    }
}