using System.Net.Http.Json;
using System.Text.Json;
using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.WebAssembly.Components;
using Congratulator.WebAssembly.Models;
using Microsoft.AspNetCore.Components;
using Timer = System.Timers.Timer;

namespace Congratulator.WebAssembly.Pages.All;

public partial class All : BasePageComponent
{
    #region Injected Services
    [Inject] 
    public IHttpClientFactory HttpClientFactory { get; set; } = null!;

    [Inject] 
    private JsonSerializerOptions JsonOptions { get; set; } = null!;
    #endregion
    
    #region State
    private int CurrentPage { get; set; } = 1;
    private string _searchQuery = string.Empty;
    private Timer? _debounceTimer;
    private string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            
            _debounceTimer?.Stop();
            _debounceTimer?.Dispose();
            
            _debounceTimer = new Timer(500);
            _debounceTimer.Elapsed += async (sender, e) => await OnSearch();
            _debounceTimer.AutoReset = false;
            _debounceTimer.Start();
        }
    }
    private string CurrentSort { get; set; } = "birthday"; // birthday, name, age
    private HashSet<RelationshipType> ActiveRelationshipTypes { get; set; } = new();
    private bool IsDescending { get; set; }
    private bool IsAddModalVisible { get; set; }
    #endregion

    #region Data
    private PagedResponse<PersonModel> People { get; set; } = new();

    private static List<RelationshipType> AvailableRelationshipTypes 
        => Enum.GetValues<RelationshipType>().ToList();
    #endregion
    
    #region Lifecycle
    protected override async Task OnInitializedAsync()
    {
        await LoadPeople();
    }
    
    private async Task LoadPeople()
    {
        var queryParams = new List<string>();

        // RelationshipTypes
        if (ActiveRelationshipTypes.Count != 0)
        {
            queryParams.AddRange(ActiveRelationshipTypes.Select(t => $"relationshipTypes={t}"));
        }

        // Search
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            queryParams.Add($"search={Uri.EscapeDataString(SearchQuery)}");
        }

        // Sort
        queryParams.Add($"sort={CurrentSort}");

        // Sort Descending
        if (IsDescending)
        {
            queryParams.Add($"sortDesc={IsDescending}");
        }

        // Always all=true
        queryParams.Add("all=true");
        
        queryParams.Add($"page={CurrentPage}");

        var queryString = string.Join("&", queryParams);
        var url = $"{Routes.Api.Persons}?{queryString}";
        
        var client = HttpClientFactory.CreateClient("ApiClient");
        People = (await client.GetFromJsonAsync<PagedResponse<PersonModel>>(url, JsonOptions, CancellationToken.None))!;

        StateHasChanged();
    }
    
    private async Task OnSearch()
    {
        await InvokeAsync(async () =>
        {
            await LoadPeople();
            StateHasChanged();
        });
    }
    
    private async Task OnPageChanged(int page)
    {
        CurrentPage = page;
        await LoadPeople();
        StateHasChanged();
    }
    #endregion

    #region Handlers
    private void OnSortChanged(ChangeEventArgs e)
    {
        CurrentSort = e.Value?.ToString() ?? "birthday";
        
        _ = LoadPeople();
    }

    private void ToggleTag(RelationshipType tag)
    {
        if (!ActiveRelationshipTypes.Remove(tag))
        {
            ActiveRelationshipTypes.Add(tag);
        }

        _ = LoadPeople();
    }
    
    private void OnToggleDescending()
    {
        IsDescending = !IsDescending;
        _ = LoadPeople();
    }

    private void OnAddClick()
    {
        IsAddModalVisible = true;
    }
    
    private async Task OnPersonAdded()
    {
        IsAddModalVisible = false;
        await LoadPeople();
        StateHasChanged();
    }

    private void OnEditClick(PersonModel person)
    {
        // TODO: Navigate to edit page or open modal
    }

    private async Task OnDeleteClick(PersonModel person)
    {
        var client = HttpClientFactory.CreateClient("ApiClient");

        var deleted = await DeleteEntityWithConfirmation(
            $"{person.FirstName} {person.LastName}",
            "Person",
            async () =>
            {
                var response = await client.DeleteAsync($"{Routes.Api.Delete}/{person.Id}", CancellationToken.None);
                return response.IsSuccessStatusCode
                    ? new SuccessfulResult()
                    : new FailureResult($"Server returned {(int)response.StatusCode}");
            });

        if (deleted)
            await LoadPeople();
        
        StateHasChanged();
    }
    #endregion

    #region Helpers
    private static string GetRowClass(PersonModel person)
    {
        return person.DaysUntilBirthday switch
        {
            0 => "row-today",
            <= 7 => "row-soon",
            _ => ""
        };
    }

    private static string GetDaysBadgeClass(int days)
    {
        return days switch
        {
            0 => "badge-today",
            <= 7 => "badge-soon",
            <= 30 => "badge-month",
            _ => "badge-default"
        };
    }

    private static string GetAgeWord(int age)
    {
        return age == 1 ? "year old" : "years old";
    }

    private static string GetDaysWord(int days)
    {
        return days == 1 ? "day" : "days";
    }
    #endregion
}