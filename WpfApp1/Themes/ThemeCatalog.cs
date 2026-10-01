namespace WpfApp1.Themes;

public static class ThemeCatalog
{
    public static IReadOnlyList<Theme> All { get; } =
    [
        new("dark", "#323437", "#212327", "#d1d0c5", "#646669", "#e2b714", "#ca4754"),
        new("light", "#fafafa", "#e6e6e6", "#2b2b2b", "#8a8a8a", "#c98a04", "#c0392b"),
        new("nord", "#2e3440", "#272c35", "#eceff4", "#7b88a1", "#88c0d0", "#bf616a"),
        new("cobalt", "#141a24", "#0e131b", "#dbe7f5", "#5f7691", "#4ea1ff", "#ff6b81"),
        new("ember", "#241a18", "#1a1210", "#f3e6e1", "#8d6f66", "#ff8a4c", "#e05a6d"),
        new("matrix", "#0b120c", "#060b07", "#c8f7c5", "#4d7a4c", "#3fae5a", "#d64545")
    ];

    public static Theme Get(string? name) =>
        All.FirstOrDefault(theme => theme.Name == name) ?? All[0];
}
