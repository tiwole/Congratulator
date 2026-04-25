using Congratulator.SharedKernel.Contracts.Enums;

namespace Congratulator.SharedKernel.Contracts.Models.Requests;

public class CreatePersonRequest
{
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? RelationshipType { get; set; } // Unknown, Friend, Mate, Coworker, Family
    public string? Photo { get; set; }
}