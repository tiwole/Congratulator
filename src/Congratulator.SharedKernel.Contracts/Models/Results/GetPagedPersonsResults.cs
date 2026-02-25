namespace Congratulator.SharedKernel.Contracts.Models.Results;

public class GetPagedPersonsResults
{
    public List<PersonModel> Data { get; set; }
    public int TotalCount { get; set; }
    public bool HasNext { get; set; }
}