using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Components.Sidebar;

public partial class Navbar : BasePageComponent
{
    [Parameter]
    [EditorRequired]
    public string Title { get; set; } = "";
    
    [Parameter] 
    [EditorRequired]
    public List<SidebarItem> Items { get; set; } = [];
}
