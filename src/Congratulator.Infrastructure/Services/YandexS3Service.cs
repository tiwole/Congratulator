using Amazon.S3;
using Amazon.S3.Model;
using Congratulator.SharedKernel.Contracts.Options;
using Congratulator.SharedKernel.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Congratulator.Infrastructure.Services;

public class YandexS3Service(IOptions<YandexS3Options> options, ILogger<YandexS3Service> logger) : IStorageService
{
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var config = new AmazonS3Config
        {
            ServiceURL = options.Value.ServiceUrl,
            AuthenticationRegion = options.Value.Region
        };

        using var client = new AmazonS3Client(options.Value.AccessKey, options.Value.SecretKey, config);

        var extension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";

        var uploadRequest = new PutObjectRequest
        {
            BucketName = options.Value.BucketName,
            Key = uniqueFileName,
            InputStream = fileStream,
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        };
        
        await client.PutObjectAsync(uploadRequest);
        
        logger.LogInformation("Successfully uploaded {FileName} to S3", uniqueFileName);

        return uniqueFileName;
    }
}