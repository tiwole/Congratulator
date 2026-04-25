using LumexUI.Services;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Components.ThemeColorToggle;

public partial class ThemeColorToggle : ComponentBase
{
    [Inject] 
    private ThemeService ThemeService { get; set; } = null!;
}