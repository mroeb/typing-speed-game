using WpfApp1.Core.Results;
using WpfApp1.Core.Typing;

namespace WpfApp1.ViewModels;

public sealed class ResultViewModel
{
    private TestResult _result = new();

    public ResultViewModel()
    {
    }

    public ResultViewModel(TestResult result, bool isPersonalBest)
    {
        _result = result;
        IsPersonalBest = isPersonalBest;
    }

    public int Wpm => _result.Wpm;

    public double Accuracy => _result.Accuracy;

    public double Consistency => _result.Consistency;

    public int TypedWords => _result.TypedWords;

    public int CorrectWords => _result.CorrectWords;

    public IReadOnlyList<SpeedSample> SpeedSamples => _result.SpeedSamples;

    public bool IsPersonalBest { get; }

    public string DurationText => FormatDuration(TimeSpan.FromSeconds(_result.DurationSeconds));

    public static string FormatDuration(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            return "0:00";
        }

        return duration.TotalHours >= 1
            ? $"{(int)duration.TotalHours}:{duration.Minutes:00}:{duration.Seconds:00}"
            : $"{(int)duration.TotalMinutes}:{duration.Seconds:00}";
    }
}
