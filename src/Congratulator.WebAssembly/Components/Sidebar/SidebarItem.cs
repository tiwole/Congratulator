using Microsoft.AspNetCore.Components.Routing;

namespace Congratulator.WebAssembly.Components.Sidebar;

public record SidebarItem(string Title, string Href, string Icon, NavLinkMatch Match = NavLinkMatch.Prefix);
