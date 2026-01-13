using Congratulator.SharedKernel.Entities;

namespace Congratulator.SharedKernel.Interfaces.Repositories;

public interface IPersonRepository
{
    public Task CreatePersonAsync(Person person);
}