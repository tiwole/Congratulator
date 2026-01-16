using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces.Repositories;

namespace Congratulator.Core.Services;

public class GetPersonsService(IPersonRepository personRepository)
{
    public async Task<GetPersonsResponse> RunAsync(GetPersonsRequest request)
    {
        return await personRepository.GetPersonsAsync(request);
    }
}