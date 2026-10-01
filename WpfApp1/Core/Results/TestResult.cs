using WpfApp1.Core.Enums;
using WpfApp1.Core.Typing;

namespace WpfApp1.Core.Results;

public sealed record TestResult
{
    public TestMode Mode { get; init; }

    public int Wpm { get; init; }

    public double Accuracy { get; init; }

    public double Consistency { get; init; }

    public int TypedWords { get; init; }

    public int CorrectWords { get; init; }

    public double DurationSeconds { get; init; }

    public DateTimeOffset CompletedAt { get; init; }

    public IReadOnlyList<SpeedSample> SpeedSamples { get; init; } = [];
}
