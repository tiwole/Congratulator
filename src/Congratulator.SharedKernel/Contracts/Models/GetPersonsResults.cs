using Congratulator.SharedKernel.Entities;

namespace Congratulator.SharedKernel.Contracts.Models;

public class GetPersonsResults
{
    public List<Person> Data { get; set; } = [];
    public bool HasMore { get; set; }
}