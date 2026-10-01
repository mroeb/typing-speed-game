namespace WpfApp1.Core.Time;

public interface IStopwatch
{
    TimeSpan Elapsed { get; }

    void Start();

    void Stop();

    void Reset();
}
