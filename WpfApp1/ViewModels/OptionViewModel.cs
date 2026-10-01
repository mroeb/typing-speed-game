namespace WpfApp1.ViewModels;

public sealed class OptionViewModel<T> : ViewModelBase
{
    private bool _isSelected;

    public OptionViewModel(T value, string label)
    {
        Value = value;
        Label = label;
    }

    public T Value { get; }

    public string Label { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
