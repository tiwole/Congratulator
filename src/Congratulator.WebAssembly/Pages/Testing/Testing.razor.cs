using System.Net.Http.Json;
using System.Text.Json;
using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.WebAssembly.Components;
using Congratulator.WebAssembly.Models;
using LumexUI;
using LumexUI.Common;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Pages.Testing;

public partial class Testing : BasePageComponent
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
    private string? _nameFilter;
    private string? _nameFilterInput;

    #endregion

    #region Handlers

    private async Task ApplyFiltersAsync()
    {
        _nameFilter = _nameFilterInput;
        await _grid.RefreshDataAsync();
    }

    private async Task SearchClick() => await ApplyFiltersAsync();
    
    private async Task KeyPressHandler(Microsoft.AspNetCore.Components.Web.KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
            await ApplyFiltersAsync();
    }

    private async Task ResetGrid()
    {
        _nameFilter = null;
        _nameFilterInput = null;
        await _grid.RefreshDataAsync();
    }

    #endregion

    #region DataSource

    private async ValueTask<DataSourceResult<PersonModel>> LoadPeopleAsync(DataSourceRequest<PersonModel> request)
    {
        IsLoading = true;
        StateHasChanged();

        var queryParams = new List<string> { "all=true" };

        // Paging
        var page = request.Count > 0 ? (request.StartIndex / request.Count) + 1 : 1;
        queryParams.Add($"page={page}");

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

        IsLoading = false;
        StateHasChanged();

        return new DataSourceResult<PersonModel>
        {
            Items = response?.Data ?? [],
            TotalItemCount = response?.TotalCount ?? 0
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

    private static ThemeColor GetChipColor(RelationshipType type)
        => type switch
        {
            RelationshipType.Friend => ThemeColor.Success,
            RelationshipType.Mate => ThemeColor.Secondary,
            RelationshipType.Coworker => ThemeColor.Primary,
            RelationshipType.Family => ThemeColor.Warning,
            /* RelationshipType.Unknown */ _ => ThemeColor.Default
        };

    #endregion
}