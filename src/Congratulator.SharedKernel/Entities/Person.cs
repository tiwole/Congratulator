using Congratulator.SharedKernel.Enums;
using Congratulator.SharedKernel.Interfaces;

namespace Congratulator.SharedKernel.Entities;

public class Person : IUniqueIdentifier
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public RelationshipType RelationshipType { get; set; } = RelationshipType.Unknown;
}