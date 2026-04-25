using Congratulator.SharedKernel.Contracts.Models;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Components.HomeSection;

public partial class HomeSection : BasePageComponent
{
    [Parameter, EditorRequired] public string Title { get; set; } = null!;
    [Parameter, EditorRequired] public List<PersonModel> People { get; set; } = null!;
}