namespace Congratulator.SharedKernel.Contracts.Models.Requests;

public class PaginatedRequest
{
    public int PageSize { get; set; } = 1024;
    public DateOnly Cursor { get; set; }
}