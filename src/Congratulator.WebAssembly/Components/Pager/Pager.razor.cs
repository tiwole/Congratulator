using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Components.Pager;

public partial class Pager : ComponentBase
{
    [Parameter] 
    public int CurrentPage { get; set; }
    
    [Parameter]
    public int TotalPages { get; set; }
    
    [Parameter]
    public EventCallback<int> OnPageChanged { get; set; }
    
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private List<int> GetPageNumbers()
    {
        var pages = new List<int>();
        
        pages.Add(1);
        
        int start = Math.Max(2, CurrentPage);
        int end = Math.Min(TotalPages - 1, CurrentPage + 2);
        
        if (start > 2)
        {
            pages.Add(-1);
        }

        for (int i = start; i <= end; i++)
        {
            pages.Add(i);
        }
        
        if (end < TotalPages - 1)
        {
            pages.Add(-1);
        }

        if (TotalPages > 1)
        {
            pages.Add(TotalPages);
        }

        return pages.Distinct().OrderBy(x => x == -1 ? int.MaxValue : x).ToList();
    }
}