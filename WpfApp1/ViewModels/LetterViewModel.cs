using WpfApp1.Core.Enums;
using WpfApp1.Core.Typing;

namespace WpfApp1.ViewModels;

public sealed class LetterViewModel : ViewModelBase
{
    private readonly Letter _letter;
    private bool _isCurrent;
    private bool _isCaretVisible;

    public LetterViewModel(Letter letter)
    {
        ArgumentNullException.ThrowIfNull(letter);

        _letter = letter;
    }

    public char Value => _letter.Value;

    public LetterState State => _letter.State;

    public bool IsCurrent
    {
        get => _isCurrent;
        set
        {
            if (SetProperty(ref _isCurrent, value))
            {
                IsCaretVisible = value;
            }
        }
    }

    public bool IsCaretVisible
    {
        get => _isCaretVisible;
        set => SetProperty(ref _isCaretVisible, value);
    }

    public void RefreshState() => OnPropertyChanged(nameof(State));
}
