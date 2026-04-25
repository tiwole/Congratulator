using System.Net.Http.Json;
using System.Text.Json;
using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.WebAssembly.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Congratulator.WebAssembly.Pages.Chart;

public record MonthData(
    int MonthIndex,
    int Total,
    Dictionary<RelationshipType, int> ByRelation,
    List<PersonModel> People);

public partial class Chart : BasePageComponent
{
    [Inject] 
    private IHttpClientFactory HttpClientFactory { get; set; } = null!;
    
    [Inject] 
    private JsonSerializerOptions JsonOptions { get; set; } = null!;

    private static readonly string[] MonthsShort =
        ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    private static readonly string[] MonthsLong =
        ["January", "February", "March", "April", "May", "June",
         "July", "August", "September", "October", "November", "December"];

    private static readonly RelationshipType[] AllRelations = Enum.GetValues<RelationshipType>();

    private bool _isLoading = true;
    private int? _hoverMonthIdx;
    private double _tooltipX;
    private double _tooltipY;

    private int _totalPeople;
    private List<MonthData> _perMonth = [];
    private double _niceMax;
    private List<double> _yTicks = [];
    private int _currentMonth;
    private string _busiestMonth = "";
    private string _quietestMonth = "";

    protected override async Task OnInitializedAsync()
    {
        _currentMonth = DateTime.Now.Month - 1;
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        _isLoading = true;

        var client = HttpClientFactory.CreateClient("ApiClient");
        var response = await client.GetFromJsonAsync<PagedResponse<PersonModel>>(
            $"{Routes.Api.Persons}?all=true&pagesize=10000", JsonOptions);

        var people = response?.Data ?? [];
        _totalPeople = people.Count;

        _perMonth = Enumerable.Range(0, 12).Select(mi =>
        {
            var monthPeople = people
                .Where(p => p.BirthDate.Month - 1 == mi)
                .OrderBy(p => p.BirthDate.Day)
                .ToList();
            var byRelation = AllRelations.ToDictionary(r => r, r => monthPeople.Count(p => p.RelationshipType == r));
            return new MonthData(mi, monthPeople.Count, byRelation, monthPeople);
        }).ToList();

        var max = _perMonth.Count > 0 ? _perMonth.Max(m => m.Total) : 0;
        _niceMax = Math.Max(5, Math.Ceiling(max / 2.0) * 2);
        _yTicks = [0, _niceMax * 0.25, _niceMax * 0.5, _niceMax * 0.75, _niceMax];

        var busiest = _perMonth.MaxBy(m => m.Total);
        var quietest = _perMonth.MinBy(m => m.Total);
        _busiestMonth = busiest is not null ? MonthsLong[busiest.MonthIndex] : "";
        _quietestMonth = quietest is not null ? MonthsLong[quietest.MonthIndex] : "";

        _isLoading = false;
    }

    private void OnBarMouseEnter(int monthIdx, MouseEventArgs e)
    {
        _hoverMonthIdx = monthIdx;
        _tooltipX = e.ClientX;
        _tooltipY = e.ClientY;
    }

    private void OnBarMouseLeave()
    {
        _hoverMonthIdx = null;
    }
}
