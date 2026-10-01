using WpfApp1.Core.Enums;

namespace WpfApp1.Core.Typing;

public sealed class Letter
{
    public Letter(char value) => Value = value;

    public char Value { get; }

    public LetterState State { get; internal set; } = LetterState.Untyped;
}
