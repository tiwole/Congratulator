using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Congratulator.WebAssembly.Components.Navbar;

public partial class Navbar : BasePageComponent
{
    [Parameter, EditorRequired] public string Title { get; set; } = "";
    [Parameter, EditorRequired] public List<NavbarItem> Items { get; set; } = [];

    private ElementReference _brandRef;

    private async Task OnBrandClick()
    {
        await base.JsRuntime.InvokeVoidAsync("launchConfetti", _brandRef);
    }
}