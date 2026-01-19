namespace Congratulator.SharedKernel.Contracts.Models;

public class GetPersonsResults
{
    public List<PersonModel> Data { get; set; } = [];
    public bool HasMore { get; set; }
}