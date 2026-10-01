using WpfApp1.Core.Enums;

namespace WpfApp1.Core.Typing;

public sealed class Word
{
    public Word(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        Text = text;
        Letters = [.. text.Select(character => new Letter(character))];
    }

    public string Text { get; }

    public IReadOnlyList<Letter> Letters { get; }

    public bool ContainsErrors => Letters.Any(letter => letter.State == LetterState.Wrong);

    public bool IsFullyTyped => Letters.All(letter => letter.State != LetterState.Untyped);
}
