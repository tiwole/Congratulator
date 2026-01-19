using Congratulator.SharedKernel.Contracts.Enums;

namespace Congratulator.SharedKernel.Contracts.Models;

public class PersonModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public RelationshipType RelationshipType { get; set; }
    // TODO: Photo path
}