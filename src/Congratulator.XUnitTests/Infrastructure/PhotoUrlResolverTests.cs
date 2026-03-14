using Congratulator.Infrastructure.Exceptions;
using Congratulator.Infrastructure.Extensions;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Options;
using Congratulator.SharedKernel.Entities;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Congratulator.XUnitTests.Infrastructure;

public class PhotoUrlResolverTests
{
    private readonly PhotoUrlResolver _resolver;

    public PhotoUrlResolverTests()
    {
        var options = Substitute.For<IOptions<YandexS3Options>>();
        options.Value.Returns(new YandexS3Options
        {
            ServiceURL = "https://storage.yandexcloud.net",
            BucketName = "my-bucket",
            AccessKey = "key",
            SecretKey = "secret",
            Region = "ru-central1"
        });
        _resolver = new PhotoUrlResolver(options);
    }

    [Fact]
    public void Resolve_WithValidPhotoPath_ReturnsFullUrl()
    {
        var person = new Person { FirstName = "John", PhotoPath = "photo123.png" };
        var model = new PersonModel();

        var result = _resolver.Resolve(person, model, null, null!);

        Assert.Equal("https://storage.yandexcloud.net/my-bucket/photo123.png", result);
    }

    [Fact]
    public void Resolve_WithEmptyPhotoPath_ThrowsNoBucketNameException()
    {
        var person = new Person { FirstName = "John", PhotoPath = "" };

        Assert.Throws<NoBucketNameException>(
            () => _resolver.Resolve(person, new PersonModel(), null, null!));
    }

    [Fact]
    public void Resolve_WithNullPhotoPath_ThrowsNoBucketNameException()
    {
        var person = new Person { FirstName = "John", PhotoPath = null };

        Assert.Throws<NoBucketNameException>(
            () => _resolver.Resolve(person, new PersonModel(), null, null!));
    }
}
