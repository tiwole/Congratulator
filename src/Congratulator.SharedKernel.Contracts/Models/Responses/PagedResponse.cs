namespace Congratulator.SharedKernel.Contracts.Models.Responses;

public class PagedResponse<T>
{
    public List<T>? Data { get; set; }
    public int TotalCount { get; set; }
    public bool HasNext { get; set; }
    public int Page { get; set; }
}