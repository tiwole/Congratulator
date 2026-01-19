using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Pages;

public partial class Home : ComponentBase
{
    [Inject] public IHttpClientFactory HttpClientFactory { get; set; }
    [Inject] JsonSerializerOptions JsonOptions { get; set; }
    
    public GetPersonsResponse? Persons { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        var client = HttpClientFactory.CreateClient("ApiClient");
        Persons = await client.GetFromJsonAsync<GetPersonsResponse>($"persons", JsonOptions, CancellationToken.None);
        StateHasChanged();
    }
}