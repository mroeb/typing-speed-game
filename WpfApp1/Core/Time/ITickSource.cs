namespace WpfApp1.Core.Time;

public interface ITickSource
{
    event EventHandler? Tick;

    void Start();

    void Stop();
}
