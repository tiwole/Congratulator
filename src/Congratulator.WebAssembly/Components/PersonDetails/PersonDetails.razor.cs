using Congratulator.SharedKernel.Contracts.Models;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Components.PersonDetails;

public partial class PersonDetails : ComponentBase
{
    [Parameter]
    public PersonModel Person { get; set; } = null!;
}