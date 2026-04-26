using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Contracts.Models;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Components.PersonDetails;

public partial class PersonDetails : BasePageComponent
{
    [Parameter]
    public PersonModel Person { get; set; } = null!;

    private string GetName(PersonModel person) => person.LastName is null ? person.FirstName : $"{person.FirstName} {person.LastName}";

    private string GetRelationshipColorVar() => Person.RelationshipType switch
    {
        RelationshipType.Family => "--lumex-warning",
        RelationshipType.Friend => "--lumex-success",
        RelationshipType.Mate => "--lumex-secondary",
        RelationshipType.Coworker => "--lumex-primary",
        _ => "--lumex-default"
    };
}