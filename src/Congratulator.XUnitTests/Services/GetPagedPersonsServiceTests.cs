using Congratulator.Core.Services;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Contracts.Models.Results;
using Congratulator.SharedKernel.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace Congratulator.XUnitTests.Services;

public class GetPagedPersonsServiceTests
{
    private readonly IPersonRepository _personRepository = Substitute.For<IPersonRepository>();
    private readonly GetPagedPersonsService _service;

    public GetPagedPersonsServiceTests()
    {
        _service = new GetPagedPersonsService(_personRepository);
    }

    [Fact]
    public async Task RunAsync_MapsDataFromRepository()
    {
        var request = new GetPersonsRequest { Page = 2 };
        var repoResult = new GetPagedPersonsResults
        {
            Data = [new PersonModel { FirstName = "John" }],
            TotalCount = 10,
            HasNext = true
        };
        _personRepository.GetPagedPersonsAsync(request).Returns(repoResult);

        var result = await _service.RunAsync(request);

        Assert.Equal(repoResult.Data, result.Data);
        Assert.Equal(10, result.TotalCount);
        Assert.True(result.HasNext);
    }

    [Fact]
    public async Task RunAsync_SetsPageFromRequest()
    {
        var request = new GetPersonsRequest { Page = 3 };
        _personRepository.GetPagedPersonsAsync(request).Returns(new GetPagedPersonsResults
        {
            Data = [],
            TotalCount = 0,
            HasNext = false
        });

        var result = await _service.RunAsync(request);

        Assert.Equal(3, result.Page);
    }

    [Fact]
    public async Task RunAsync_WhenNoResults_ReturnsEmptyWithCorrectPage()
    {
        var request = new GetPersonsRequest { Page = 1 };
        _personRepository.GetPagedPersonsAsync(request).Returns(new GetPagedPersonsResults
        {
            Data = [],
            TotalCount = 0,
            HasNext = false
        });

        var result = await _service.RunAsync(request);

        Assert.Empty(result.Data);
        Assert.Equal(0, result.TotalCount);
        Assert.False(result.HasNext);
        Assert.Equal(1, result.Page);
    }
}