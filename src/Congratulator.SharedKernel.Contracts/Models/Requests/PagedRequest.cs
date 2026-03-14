namespace Congratulator.SharedKernel.Contracts.Models.Requests;

public class PagedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 8;
}