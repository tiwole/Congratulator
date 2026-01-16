using Congratulator.SharedKernel.Entities;

namespace Congratulator.SharedKernel.Contracts.Models.Responses;

public class GetPersonsResponse
{
    public List<Person> TodayBirthdays { get; set; } = [];
    public List<Person> UpcomingBirthdays { get; set; } = [];
}