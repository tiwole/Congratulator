using Congratulator.SharedKernel.Contracts.Enums;

namespace Congratulator.SharedKernel.Contracts.Models.Requests;

public class GetPersonsRequest
{
    public int? Upcoming { get; set; } = 3;
    public RelationshipType? Status { get; set; }
    public string? Search { get; set; }
    public bool? All { get; set; } = false;
}