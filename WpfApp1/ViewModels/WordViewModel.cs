using WpfApp1.Core.Typing;

namespace WpfApp1.ViewModels;

public sealed class WordViewModel : ViewModelBase
{
    private bool _isAwaitingSeparator;

    public WordViewModel(Word word)
    {
        ArgumentNullException.ThrowIfNull(word);

        Letters = [.. word.Letters.Select(letter => new LetterViewModel(letter))];
    }

    public IReadOnlyList<LetterViewModel> Letters { get; }

    public bool IsAwaitingSeparator
    {
        get => _isAwaitingSeparator;
        set => SetProperty(ref _isAwaitingSeparator, value);
    }
}
