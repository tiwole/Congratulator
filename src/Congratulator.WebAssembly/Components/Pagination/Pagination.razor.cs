using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Components.Pagination;

public partial class Pagination : ComponentBase
{
    [Parameter]
    public int CurrentPage { get; set; }

    [Parameter]
    public int TotalPages { get; set; }

    [Parameter]
    public EventCallback<int> OnPageChanged { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private bool IsFirstPage => CurrentPage <= 1;
    private bool IsLastPage => CurrentPage >= TotalPages;

    private async Task GoToPage(int page)
    {
        if (page < 1 || page > TotalPages || page == CurrentPage)
            return;

        await OnPageChanged.InvokeAsync(page);
    }

    private List<int> GetPageNumbers()
    {
        var pages = new List<int> { 1 };

        var start = Math.Max(2, CurrentPage - 1);
        var end = Math.Min(TotalPages - 1, CurrentPage + 1);

        if (start > 2)
            pages.Add(-1);

        for (var i = start; i <= end; i++)
            pages.Add(i);

        if (end < TotalPages - 1)
            pages.Add(-1);

        if (TotalPages > 1)
            pages.Add(TotalPages);

        return pages;
    }
}