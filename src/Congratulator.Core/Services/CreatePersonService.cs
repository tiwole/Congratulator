using Congratulator.Core.Exceptions;
using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces.Repositories;
using Congratulator.SharedKernel.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Congratulator.Core.Services;

public class CreatePersonService(IPersonRepository personRepository, IStorageService storageService, ILogger<CreatePersonService> logger)
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

        if (!string.IsNullOrEmpty(request.Photo))
        {
            try
            {
                using var stream = new MemoryStream(Convert.FromBase64String(request.Photo));
                var fileName = $"{Guid.NewGuid()}.png";
                person.PhotoPath = await storageService.UploadFileAsync(stream, fileName, "image/png");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new ImageException($"Error during uploading image: {e.Message}");
            }
        }

        await personRepository.CreatePersonAsync(person);

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