using Congratulator.Core.Exceptions;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.SharedKernel.Interfaces.Repositories;
using Congratulator.SharedKernel.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Congratulator.Core.Services;

public class UpdatePersonService(IPersonRepository personRepository, IStorageService storageService, ILogger<UpdatePersonService> logger)
{
    public async Task<CreatePersonResponse> RunAsync(Guid personId, UpdatePersonRequest request)
    {
        var person = await personRepository.GetPersonByIdAsync(personId)
                     ?? throw new PersonNotFoundException("Person not found");

        bool hasChanges = false;
        
        if (!string.IsNullOrEmpty(request.Photo))
        {
            try
            {
                if (person.PhotoPath != null && person.PhotoPath != "default.png")
                    await storageService.DeleteFileAsync(person.PhotoPath);
                
                using var stream = new MemoryStream(Convert.FromBase64String(request.Photo));
                var fileName = $"{Guid.NewGuid()}.png";
                person.PhotoPath = await storageService.UploadFileAsync(stream, fileName, "image/png");
            }
            catch (Exception e)
            {
                logger.LogError("Error during updating image: {error}", e.Message);
                throw new ImageException($"Error during updating image: {e.Message}");
            }
                
        }

        if (request.FirstName is { } fn && fn != person.FirstName)
            (person.FirstName, hasChanges) = (fn, true);

        if (request.LastName is { } ln && ln != person.LastName)
            (person.LastName, hasChanges) = (ln, true);

        if (request.BirthDate is { } bd && bd != person.BirthDate)
            (person.BirthDate, hasChanges) = (bd, true);

        if (request.RelationshipType is { } rt && rt != person.RelationshipType)
            (person.RelationshipType, hasChanges) = (rt, true);
        
        if (request.Photo is { } url && url != person.PhotoPath)
            (person.PhotoPath, hasChanges) = (url, true);

        if (hasChanges)
        {
            await personRepository.UpdatePersonAsync(person);
            
            logger.LogInformation("Person {PersonId} updated", person.Id);
        }

        return new CreatePersonResponse
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            BirthDate = person.BirthDate,
            RelationshipType = person.RelationshipType,
            PhotoPath = person.PhotoPath
        };
    }
}