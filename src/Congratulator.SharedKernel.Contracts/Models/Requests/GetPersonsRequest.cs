using Congratulator.SharedKernel.Contracts.Enums;

namespace Congratulator.SharedKernel.Contracts.Models.Requests;

public class GetPersonsRequest : PagedRequest
{
    public int? Upcoming { get; set; } = 64;
    public bool? All { get; set; } = false;
    
    
    public List<RelationshipType>? RelationshipTypes { get; set; }
    public string? Search { get; set; }
    public SortVariants? Sort { get; set; }
    public bool? SortDesc { get; set; }
}