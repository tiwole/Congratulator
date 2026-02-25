using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.SharedKernel.Interfaces.Repositories;

namespace Congratulator.Core.Services;

public class GetPagedPersonsService(IPersonRepository personRepository)
{
    public async Task<PagedResponse<PersonModel>> RunAsync(GetPersonsRequest request)
    {
        var data = await personRepository.GetPagedPersonsAsync(request);
        var result = new PagedResponse<PersonModel>
        {
            Data = data.Data,
            TotalCount = data.TotalCount,
            HasNext = data.HasNext,
            Page = request.Page
        };
        return result;
    }
}