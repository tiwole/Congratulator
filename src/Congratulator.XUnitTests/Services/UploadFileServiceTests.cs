using Congratulator.Core.Services;
using Congratulator.SharedKernel.Interfaces.Services;
using NSubstitute;
using Xunit;

namespace Congratulator.XUnitTests.Services;

public class UploadFileServiceTests
{
    private readonly IStorageService _storageService = Substitute.For<IStorageService>();
    private readonly UploadFileService _service;

    public UploadFileServiceTests()
    {
        _service = new UploadFileService(_storageService);
    }

    [Fact]
    public async Task UploadFileAsync_DelegatesToStorageService()
    {
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        _storageService.UploadFileAsync(stream, "test.png", "image/png").Returns("result.png");

        var result = await _service.UploadFileAsync(stream, "test.png", "image/png");

        Assert.Equal("result.png", result);
        await _storageService.Received(1).UploadFileAsync(stream, "test.png", "image/png");
    }
}