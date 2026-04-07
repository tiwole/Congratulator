using AutoMapper;
using Congratulator.Infrastructure.Exceptions;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Options;
using Congratulator.SharedKernel.Entities;
using Microsoft.Extensions.Options;

namespace Congratulator.Infrastructure.Extensions;

public class PhotoUrlResolver(IOptions<YandexS3Options> options) : IValueResolver<Person, PersonModel, string?>
{
    public string Resolve(Person source, PersonModel destination, string? destMember, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source.PhotoPath))
        {
            throw new NoBucketNameException("Bucket name in appsettings.json is not specified");
        }
        
        return $"{options.Value.ServiceUrl}/{options.Value.BucketName}/{source.PhotoPath}";
    }
}