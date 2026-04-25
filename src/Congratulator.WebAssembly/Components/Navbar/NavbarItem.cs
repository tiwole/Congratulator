using Microsoft.AspNetCore.Components.Routing;

namespace Congratulator.WebAssembly.Components.Navbar;

public record NavbarItem(string Title, string Href, string Icon, NavLinkMatch Match = NavLinkMatch.Prefix);
