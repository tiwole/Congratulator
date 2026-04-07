using Congratulator.Core.Exceptions;
using Congratulator.Core.Services;
using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Entities;
using Congratulator.SharedKernel.Interfaces.Repositories;
using Congratulator.SharedKernel.Interfaces.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Congratulator.XUnitTests.Services;

public class CreatePersonServiceTests
{
    private readonly IPersonRepository _personRepository = Substitute.For<IPersonRepository>();
    private readonly IStorageService _storageService = Substitute.For<IStorageService>();
    private readonly ILogger<CreatePersonService> _logger = Substitute.For<ILogger<CreatePersonService>>();
    private readonly CreatePersonService _service;

    public CreatePersonServiceTests()
    {
        _service = new CreatePersonService(_personRepository, _storageService, _logger);
    }

    [Fact]
    public async Task RunAsync_WithoutPhoto_CreatesPersonWithoutUpload()
    {
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = new DateOnly(1990, 1, 1),
            RelationshipType = RelationshipType.Friend
        };

        var result = await _service.RunAsync(request);

        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal(new DateOnly(1990, 1, 1), result.BirthDate);
        Assert.Equal(RelationshipType.Friend, result.RelationshipType);
        await _personRepository.Received(1).CreatePersonAsync(Arg.Any<Person>());
        await _storageService.DidNotReceive().UploadFileAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task RunAsync_WithNullRelationshipType_DefaultsToUnknown()
    {
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            BirthDate = new DateOnly(1990, 1, 1),
            RelationshipType = null
        };

        var result = await _service.RunAsync(request);

        Assert.Equal(RelationshipType.Unknown, result.RelationshipType);
    }

    [Fact]
    public async Task RunAsync_WithExplicitRelationshipType_PreservesIt()
    {
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            BirthDate = new DateOnly(1990, 1, 1),
            RelationshipType = RelationshipType.Family
        };

        var result = await _service.RunAsync(request);

        Assert.Equal(RelationshipType.Family, result.RelationshipType);
    }

    [Fact]
    public async Task RunAsync_WithValidBase64Photo_UploadsToStorage()
    {
        var base64Photo = Convert.ToBase64String(new byte[] { 1, 2, 3 });
        _storageService.UploadFileAsync(Arg.Any<Stream>(), Arg.Any<string>(), "image/png")
            .Returns("uploaded-photo.png");

        var request = new CreatePersonRequest
        {
            FirstName = "John",
            BirthDate = new DateOnly(1990, 1, 1),
            Photo = base64Photo
        };

        var result = await _service.RunAsync(request);

        Assert.Equal("uploaded-photo.png", result.PhotoPath);
        await _storageService.Received(1).UploadFileAsync(Arg.Any<Stream>(), Arg.Any<string>(), "image/png");
    }

    [Fact]
    public async Task RunAsync_WithInvalidBase64Photo_ThrowsImageException()
    {
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            BirthDate = new DateOnly(1990, 1, 1),
            Photo = "not-valid-base64!!!"
        };

        await Assert.ThrowsAsync<ImageException>(() => _service.RunAsync(request));
    }

    [Fact]
    public async Task RunAsync_WhenStorageServiceThrows_ThrowsImageException()
    {
        var base64Photo = Convert.ToBase64String(new byte[] { 1, 2, 3 });
        _storageService.UploadFileAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsync(new Exception("S3 error"));

        var request = new CreatePersonRequest
        {
            FirstName = "John",
            BirthDate = new DateOnly(1990, 1, 1),
            Photo = base64Photo
        };

        var ex = await Assert.ThrowsAsync<ImageException>(() => _service.RunAsync(request));
        Assert.Contains("S3 error", ex.Message);
    }

    [Fact]
    public async Task RunAsync_WithEmptyPhoto_DoesNotUpload()
    {
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            BirthDate = new DateOnly(1990, 1, 1),
            Photo = ""
        };

        await _service.RunAsync(request);

        await _storageService.DidNotReceive().UploadFileAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task RunAsync_ReturnsResponseWithCorrectId()
    {
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            BirthDate = new DateOnly(1990, 1, 1)
        };

        var result = await _service.RunAsync(request);

        Assert.NotEqual(Guid.Empty, result.Id);
    }
}