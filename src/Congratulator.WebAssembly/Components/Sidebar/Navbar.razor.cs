using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Components.Sidebar;

public partial class Navbar : BasePageComponent
{
    [Parameter] public string Title { get; set; } = "";
    [Parameter] public List<SidebarItem> Items { get; set; } = [];
}
