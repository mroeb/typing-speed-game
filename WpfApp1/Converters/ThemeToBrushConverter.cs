using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using WpfApp1.Themes;

namespace WpfApp1.Converters;

public sealed class ThemeToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Theme theme)
        {
            return Brushes.Transparent;
        }

        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(theme.Accent));
        brush.Freeze();
        return brush;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
