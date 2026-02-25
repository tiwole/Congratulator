namespace Congratulator.SharedKernel.Contracts.Models.Responses;

public class GetPersonsResponse
{
    public List<PersonModel> TodayBirthdays { get; set; } = [];
    public List<PersonModel> UpcomingBirthdays { get; set; } = [];
}