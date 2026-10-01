using System.Diagnostics;
using WpfApp1.Core.Time;

namespace WpfApp1.Services;

public sealed class SystemStopwatch : IStopwatch
{
    private readonly Stopwatch _stopwatch = new();

    public TimeSpan Elapsed => _stopwatch.Elapsed;

    public void Start() => _stopwatch.Start();

    public void Stop() => _stopwatch.Stop();

    public void Reset() => _stopwatch.Reset();
}
