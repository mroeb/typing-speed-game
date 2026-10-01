using System.Windows;
using System.Windows.Media;

namespace WpfApp1.Themes;

public sealed class ThemeService : IThemeService
{
    private const string BackgroundKey = "BackgroundBrush";
    private const string SurfaceKey = "SurfaceBrush";
    private const string ForegroundKey = "ForegroundBrush";
    private const string SubtleKey = "SubtleBrush";
    private const string AccentKey = "AccentBrush";
    private const string ErrorKey = "ErrorBrush";

    public void Apply(Theme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        var resources = Application.Current.Resources;

        resources[BackgroundKey] = CreateBrush(theme.Background);
        resources[SurfaceKey] = CreateBrush(theme.Surface);
        resources[ForegroundKey] = CreateBrush(theme.Foreground);
        resources[SubtleKey] = CreateBrush(theme.Subtle);
        resources[AccentKey] = CreateBrush(theme.Accent);
        resources[ErrorKey] = CreateBrush(theme.Error);
    }

    private static SolidColorBrush CreateBrush(string color)
    {
        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        brush.Freeze();
        return brush;
    }
}
