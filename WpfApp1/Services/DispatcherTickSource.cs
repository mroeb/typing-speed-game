using System.Windows.Threading;
using WpfApp1.Core.Time;

namespace WpfApp1.Services;

public sealed class DispatcherTickSource : ITickSource
{
    private readonly DispatcherTimer _timer;

    public DispatcherTickSource() : this(TimeSpan.FromMilliseconds(100)) { }

    public DispatcherTickSource(TimeSpan interval)
    {
        _timer = new DispatcherTimer(interval, DispatcherPriority.Background, OnTick, Dispatcher.CurrentDispatcher);
    }

    public event EventHandler? Tick;

    public void Start() => _timer.Start();

    public void Stop() => _timer.Stop();

    private void OnTick(object? sender, EventArgs e) => Tick?.Invoke(this, e);
}
