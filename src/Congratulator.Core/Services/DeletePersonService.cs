using Congratulator.Core.Exceptions;
using Congratulator.SharedKernel.Interfaces.Repositories;
using Congratulator.SharedKernel.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Congratulator.Core.Services;

public class DeletePersonService(IPersonRepository personRepository, IStorageService storageService, ILogger<CreatePersonService> logger)
{
    public async Task RunAsync(Guid personId)
    {
        var person = await personRepository.GetPersonByIdAsync(personId)
                     ?? throw new PersonNotFoundException("Person not found");
        
        if (person.PhotoPath != null && person.PhotoPath != "default.png")
            await storageService.DeleteFileAsync(person.PhotoPath);

        await personRepository.DeletePersonAsync(person);
        
        logger.LogInformation("Person {PersonId} deleted", person.Id);
    }
}