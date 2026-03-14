using Congratulator.Core.Services;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.SharedKernel.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace Congratulator.XUnitTests.Services;

public class GetPersonsServiceTests
{
    private readonly IPersonRepository _personRepository = Substitute.For<IPersonRepository>();
    private readonly GetPersonsService _service;

    public GetPersonsServiceTests()
    {
        _service = new GetPersonsService(_personRepository);
    }

    [Fact]
    public async Task RunAsync_ReturnsRepositoryResult()
    {
        var request = new GetPersonsRequest();
        var expectedResponse = new GetPersonsResponse
        {
            TodayBirthdays = [new PersonModel { FirstName = "John" }],
            UpcomingBirthdays = [new PersonModel { FirstName = "Jane" }]
        };
        _personRepository.GetPersonsAsync(request).Returns(expectedResponse);

        var result = await _service.RunAsync(request);

        Assert.Same(expectedResponse, result);
    }

    [Fact]
    public async Task RunAsync_PassesRequestToRepository()
    {
        var request = new GetPersonsRequest { Upcoming = 5, Search = "test" };
        _personRepository.GetPersonsAsync(request).Returns(new GetPersonsResponse());

        await _service.RunAsync(request);

        await _personRepository.Received(1).GetPersonsAsync(request);
    }
}