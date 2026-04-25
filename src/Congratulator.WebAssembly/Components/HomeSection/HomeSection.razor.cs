using Congratulator.SharedKernel.Contracts.Models;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Components.HomeSection;

public partial class HomeSection : BasePageComponent
{
    [Parameter, EditorRequired] public string Title { get; set; } = null!;
    [Parameter] public List<PersonModel> People { get; set; } = [];
    [Parameter] public bool IsLoading { get; set; }

    private readonly int _skeletonCount = Random.Shared.Next(1, 7);
}