using System.Net.Http.Json;
using System.Text.Json;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Models.Responses;
using Congratulator.WebAssembly.Components;
using Congratulator.WebAssembly.Components.AddPersonModal;
using Congratulator.WebAssembly.Components.PersonModal;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Pages.Home;

public partial class Home : BasePageComponent
{
    [Inject] public IHttpClientFactory HttpClientFactory { get; set; } = null!;

    [Inject] private JsonSerializerOptions JsonOptions { get; set; } = null!;

    private bool _isLoading = true;
    private int _totalCount;
    private List<PersonModel> _today = [];
    private List<PersonModel> _thisWeek = [];
    private List<PersonModel> _thisMonth = [];
    private List<PersonModel> _upcoming = [];

    private PersonModal _personModal = null!;

    private static readonly string[] SadEmojis =
    [
        "(ノ_<。)", "(μ_μ)", "o(TヘTo)", "o(〒﹏〒)o", "(｡T ω T｡)", "(>_<)", 
        "(｡•́︿•̀｡)", "(╥_╥)", "(╥﹏╥)", "(っ˘̩╭╮˘̩)っ", "(ಡ‸ಡ)", "(ﾉД`)", "(ಥ﹏ಥ)"
    ];

    private async Task OpenPersonModal(PersonModel person)
    {
        await _personModal.Open(person);
    }

    protected override async Task OnInitializedAsync()
    {
        var client = HttpClientFactory.CreateClient("ApiClient");
        var response = await client.GetFromJsonAsync<GetPersonsResponse>(Routes.Api.Persons, JsonOptions);

        var people = response?.People ?? [];
        _totalCount = people.Count;
        _today = people.Where(p => p.DaysUntilBirthday == 0).ToList();
        _thisWeek = people.Where(p => p.DaysUntilBirthday is >= 1 and <= 7).ToList();
        _thisMonth = people.Where(p => p.DaysUntilBirthday is >= 8 and <= 30).ToList();
        _upcoming = people.Where(p => p.DaysUntilBirthday > 30).ToList();

        _isLoading = false;
    }
}