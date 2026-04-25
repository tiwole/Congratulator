using Congratulator.SharedKernel.Contracts.Models;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Components.PersonCard;

public partial class PersonCard : BasePageComponent
{
    [Parameter] 
    public PersonModel Person { get; set; } = null!;
}