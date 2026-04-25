using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Congratulator.WebAssembly.Components.Sidebar;

public partial class Navbar : BasePageComponent
{
    [Parameter, EditorRequired] public string Title { get; set; } = "";
    [Parameter, EditorRequired] public List<SidebarItem> Items { get; set; } = [];

    private ElementReference _brandRef;

    private async Task OnBrandClick()
    {
        await JsRuntime.InvokeVoidAsync("launchConfetti", _brandRef);
    }
}