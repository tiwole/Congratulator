using System.Net.Http.Json;
using System.Text.Json;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.WebAssembly.Components;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Pages.Home;

public partial class Home : BasePageComponent
{
    #region Dependencies
    [Inject] 
    public IHttpClientFactory HttpClientFactory { get; set; } = null!;

    [Inject] 
    private JsonSerializerOptions JsonOptions { get; set; } = null!;
    #endregion
    
    #region Properties
    private GetPersonsResponse? Persons { get; set; }
    #endregion
    
    #region Lifecycle
    protected override async Task OnInitializedAsync()
    {
        var client = HttpClientFactory.CreateClient("ApiClient");
        Persons = await client.GetFromJsonAsync<GetPersonsResponse>(Routes.Api.Persons, JsonOptions, CancellationToken.None);
        StateHasChanged();
    }
    #endregion
}