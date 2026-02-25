using Congratulator.Core.Exceptions;
using Congratulator.SharedKernel.Interfaces.Repositories;

namespace Congratulator.Core.Services;

public class DeletePersonService(IPersonRepository personRepository)
{
    public async Task RunAsync(Guid personId)
    {
        var person = await personRepository.GetPersonByIdAsync(personId) 
                     ?? throw new PersonNotFoundException("Person not found");

        await personRepository.DeletePersonAsync(person);
    }
}