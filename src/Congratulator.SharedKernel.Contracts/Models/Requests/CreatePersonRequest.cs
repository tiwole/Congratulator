using Congratulator.SharedKernel.Contracts.Enums;

namespace Congratulator.SharedKernel.Contracts.Models.Requests;

public class CreatePersonRequest
{
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public RelationshipType? RelationshipType { get; set; }
}