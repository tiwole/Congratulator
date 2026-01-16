using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.SharedKernel.Entities;

namespace Congratulator.SharedKernel.Interfaces.Repositories;

public interface IPersonRepository
{
    public Task CreatePersonAsync(Person person);
    public Task<Person?> GetPersonByIdAsync(Guid id);
    public Task UpdatePersonAsync(Person person);
    public Task DeletePersonAsync(Person person);
    public Task<GetPersonsResponse> GetPersonsAsync(GetPersonsRequest request);
}