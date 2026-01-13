using Congratulator.SharedKernel.Enums;

namespace Congratulator.SharedKernel.Contracts.Models.Responses;

public class CreatePersonResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public RelationshipType RelationshipType { get; set; }
}