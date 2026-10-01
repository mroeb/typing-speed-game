namespace WpfApp1.Core.Text;

public sealed class RandomWordGenerator : IWordGenerator
{
    private const int NumberOdds = 9;
    private const int PunctuationOdds = 11;
    private const int MaxNumberLength = 4;

    private static readonly char[] PunctuationMarks = [',', '.', '?', '!', ';', ':', '\'', '-', '"', ')'];

    private readonly IReadOnlyList<string> _vocabulary;
    private readonly Random _random;

    public RandomWordGenerator(IReadOnlyList<string> vocabulary) : this(vocabulary, Random.Shared) { }

    public RandomWordGenerator(IReadOnlyList<string> vocabulary, Random random)
    {
        ArgumentNullException.ThrowIfNull(vocabulary);

        if (vocabulary.Count == 0)
        {
            throw new ArgumentException("The vocabulary must not be empty.", nameof(vocabulary));
        }

        _vocabulary = vocabulary;
        _random = random;
    }

    public IReadOnlyList<string> Generate(int count, WordOptions options)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        var words = new string[count];

        for (var index = 0; index < count; index++)
        {
            words[index] = CreateWord(options, isLast: index == count - 1);
        }

        return words;
    }

    private string CreateWord(WordOptions options, bool isLast)
    {
        if (options.Numbers && _random.Next(NumberOdds) == 0)
        {
            return CreateNumber();
        }

        var word = _vocabulary[_random.Next(_vocabulary.Count)];

        if (options.Punctuation && !isLast && _random.Next(PunctuationOdds) == 0)
        {
            return word + PunctuationMarks[_random.Next(PunctuationMarks.Length)];
        }

        return word;
    }

    private string CreateNumber()
    {
        var length = _random.Next(1, MaxNumberLength + 1);
        var digits = new char[length];
        digits[0] = (char)('1' + _random.Next(9));

        for (var index = 1; index < length; index++)
        {
            digits[index] = (char)('0' + _random.Next(10));
        }

        return new string(digits);
    }
}
