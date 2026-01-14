namespace Congratulator.SharedKernel.Contracts.Models.Responses;

public class PaginatedResponse<TData>
{
    public IReadOnlyList<TData> Data { get; set; } = [];
    public bool HasMore { get; set; }
    public DateOnly? NextCursor { get; set; }
}