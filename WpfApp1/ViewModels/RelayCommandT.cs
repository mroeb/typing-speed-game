using System.Windows.Input;

namespace WpfApp1.ViewModels;

public sealed class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;

    public RelayCommand(Action<T?> execute)
    {
        ArgumentNullException.ThrowIfNull(execute);

        _execute = execute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => _execute(Cast(parameter));

    private static T? Cast(object? parameter) => parameter is T value ? value : default;
}
