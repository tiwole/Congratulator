using LumexUI.Common;
using LumexUI.Services;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Components.ThemeColorToggle;

public partial class ThemeColorToggle : ComponentBase
{
    [Inject] 
    private ThemeService ThemeService { get; set; } = null!;

    private async Task ToggleTheme(bool value) 
        => await ThemeService.SetThemeAsync(value ? Theme.Light : Theme.Dark);
}