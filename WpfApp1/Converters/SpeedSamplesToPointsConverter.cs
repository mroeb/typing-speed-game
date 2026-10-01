using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using WpfApp1.Core.Typing;

namespace WpfApp1.Converters;

public sealed class SpeedSamplesToPointsConverter : IValueConverter
{
    public double Width { get; set; } = 560;

    public double Height { get; set; } = 110;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IReadOnlyList<SpeedSample> samples || samples.Count == 0)
        {
            return new PointCollection();
        }

        var lastSample = samples[^1];
        var duration = Math.Max(lastSample.Seconds, 1);
        var peak = Math.Max(samples.Max(sample => sample.Wpm), 20) * 1.15;
        var points = new PointCollection();

        foreach (var sample in samples)
        {
            var x = sample.Seconds / duration * Width;
            var y = Height - sample.Wpm / peak * Height;
            points.Add(new Point(x, Math.Clamp(y, 0, Height)));
        }

        points.Add(new Point(Width, Math.Clamp(Height - lastSample.Wpm / peak * Height, 0, Height)));

        return points;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
