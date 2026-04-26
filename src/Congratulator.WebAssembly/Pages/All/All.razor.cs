using System.Net.Http.Json;
using System.Text.Json;
using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.WebAssembly.Components;
using Congratulator.WebAssembly.Components.PersonModal;
using Congratulator.WebAssembly.Models;
using LumexUI;
using LumexUI.Common;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Pages.All;

public partial class All : BasePageComponent
{
    private static readonly Dictionary<string, string> SortPropertyMap = new()
    {
        ["FirstName"] = "name",
        ["BirthDate"] = "birthday",
        ["Age"] = "age",
    };

    #region Injected Services

    [Inject] public IHttpClientFactory HttpClientFactory { get; set; } = null!;

    [Inject] private JsonSerializerOptions JsonOptions { get; set; } = null!;

    #endregion

    #region State

    private bool IsLoading { get; set; }
    private DataSource<PersonModel> PeopleProvider => LoadPeopleAsync;

    #endregion
    
    #region Fields
    
    private LumexDataGrid<PersonModel> _grid = null!;
    private PersonModal _personModal = null!;
    private string? _nameFilter;
    private string? _nameFilterInput;
    private int _currentPage = 1;
    private int _totalPages = 1;
    private const int PageSize = 8;
    private int _totalCount;

    private bool _isDropdownOpened;
    private HashSet<RelationshipType> ActiveRelationshipTypes { get; set; } = [];
    private static List<RelationshipType> AvailableRelationshipTypes 
        => Enum.GetValues<RelationshipType>().ToList();
    
    private static readonly string[] DissatisfactionEmojis =
    [
        "(；￣Д￣)", "(￢_￢;)", "o(TヘTo)", "o(〒﹏〒)o", "(￣︿￣)", "(>_<)", "(」＞ ＜)」",
        "(｡•́︿•̀｡)", "(ᗒᗣᗕ)՞", "(︶︹︺)", "(＞﹏＜)", "(⇀‸↼‶)", "(눈_눈)", "(--_--)"
    ];

    private string _currentEmoji = ResetEmoji();

    private int _gridKey;

    #endregion

    #region Handlers
    
    private static string ResetEmoji() => DissatisfactionEmojis[Random.Shared.Next(0, DissatisfactionEmojis.Length)];

    private async Task ApplyFiltersAsync()
    {
        _nameFilter = _nameFilterInput;
        _currentPage = 1;
        await _grid.RefreshDataAsync();
    }

    private async Task SearchClick() => await ApplyFiltersAsync();

    private async Task OnRowClickHandler(DataGridRowClickEventArgs<PersonModel> args)
    {
        await _personModal.Open(args.Item);
    }
    
    private async Task KeyPressHandler(Microsoft.AspNetCore.Components.Web.KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
            await ApplyFiltersAsync();
    }

    private bool _isRefreshSpinning;

    private async Task OnResetClick()
    {
        _isRefreshSpinning = true;
        await ResetGrid();
        await Task.Delay(1000);
        _isRefreshSpinning = false;
    }

    private async Task ResetGrid()
    {
        _nameFilter = null;
        _nameFilterInput = null;
        ActiveRelationshipTypes.Clear();
        _currentPage = 1;
        _gridKey++; // Recreate the grid, resetting its state
        await _grid.RefreshDataAsync();
    }

    private async Task OnPageChangedAsync(int page)
    {
        _currentPage = page;
        await _grid.RefreshDataAsync();
    }

    private async Task OnDropdownStateChanged(bool value)
    {
        _isDropdownOpened = value;

        if (!_isDropdownOpened)
        {
            await _grid.RefreshDataAsync();
        }
    }

    #endregion

    #region DataSource

    private async ValueTask<DataSourceResult<PersonModel>> LoadPeopleAsync(DataSourceRequest<PersonModel> request)
    {
        IsLoading = true;
        StateHasChanged();

        var queryParams = new List<string> { "all=true" };

        // Paging
        queryParams.Add($"page={_currentPage}");
        
        // RelationshipTypes
        if (ActiveRelationshipTypes.Count != 0)
        {
            queryParams.AddRange(ActiveRelationshipTypes.Select(t => $"relationshipTypes={t}"));
        }

        // Sort
        var sortDescriptors = request.GetSortDescriptors();
        if (sortDescriptors.Any())
        {
            var sort = sortDescriptors.First();
            if (SortPropertyMap.TryGetValue(sort.PropertyName, out var apiSort))
            {
                queryParams.Add($"sort={apiSort}");
                if (sort.Direction == SortDirection.Descending)
                    queryParams.Add("sortDesc=true");
            }
        }
        
        // Search
        if (!string.IsNullOrEmpty(_nameFilter))
        {
            queryParams.Add($"search={_nameFilter}");
        }

        var url = $"{Routes.Api.Persons}?{string.Join("&", queryParams)}";
        var client = HttpClientFactory.CreateClient("ApiClient");
        var response = await client.GetFromJsonAsync<PagedResponse<PersonModel>>(url, JsonOptions, request.CancellationToken);
        
        _totalCount = response?.TotalCount ?? 0;
        _totalPages = Math.Max(1, (int)Math.Ceiling((double)_totalCount / PageSize));

        if (_totalCount == 0)
            _currentEmoji = ResetEmoji();

        IsLoading = false;
        StateHasChanged();

        return new DataSourceResult<PersonModel>
        {
            Items = response?.Data ?? [],
            TotalItemCount = _totalCount
        };
    }
    
    private async Task OnDeleteClick(PersonModel person)
    {
        var client = HttpClientFactory.CreateClient("ApiClient");

        var deleted = await DeleteEntity(
            $"{person.FirstName} {person.LastName}",
            async () =>
            {
                var response = await client.DeleteAsync($"{Routes.Api.Delete}/{person.Id}", CancellationToken.None);
                return response.IsSuccessStatusCode
                    ? new SuccessfulResult()
                    : new FailureResult($"Server returned {(int)response.StatusCode}");
            });

        if (deleted)
            await _grid.RefreshDataAsync();
    }
    
    private void ToggleRelationship(RelationshipType type)
    {
        if (!ActiveRelationshipTypes.Remove(type))
            ActiveRelationshipTypes.Add(type);
    }

    #endregion

    #region Helpers

    private static string GetAgeWord(int age)
        => age == 1 ? "year old" : "years old";

    private static string GetDaysWord(int days)
        => days == 1 ? "day" : "days";

    private static string GetDaysCell(PersonModel person)
    {
        return person.DaysUntilBirthday == 0
            ? "Today!"
            : $"{person.DaysUntilBirthday} {GetDaysWord(person.DaysUntilBirthday)}";
    }

    private string GetDropdownLabel(int selectedCount)
        => selectedCount switch
        {
            0 => "Relationships",
            1 => ActiveRelationshipTypes.First().ToString(),
            _ => $"{selectedCount} Selected"
        };

    #endregion
}