using Congratulator.SharedKernel.Interfaces.Services;

namespace Congratulator.Core.Services;

public class UploadFileService(IStorageService storageService)
{
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        return await storageService.UploadFileAsync(fileStream, fileName, contentType);
    }
}