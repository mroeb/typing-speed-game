using WpfApp1.Core.Enums;
using WpfApp1.Core.Results;
using WpfApp1.Core.Settings;
using WpfApp1.Core.Text;
using WpfApp1.Core.Time;

namespace WpfApp1.Core.Typing;

public sealed class TypingTest
{
    private const int TimeModeBufferSize = 60;
    private const int TimeModeAppendSize = 20;
    private const int TimeModeBufferThreshold = 25;
    private const double SampleIntervalSeconds = 1;
    private const double MinimumSampleSeconds = 0.05;
    private const double MinutesPerMinute = 1;
    private const double MinutesPerSecond = 1 / 60.0;
    private const double WordsPerMinutePerCharacter = 12;

    private readonly IStopwatch _stopwatch;
    private readonly IWordGenerator _wordGenerator;
    private readonly GameSettings _settings;
    private readonly WordOptions _wordOptions;
    private readonly TimeSpan _timeLimit;
    private readonly List<Word> _words = [];
    private readonly List<double> _keystrokeSpeeds = [];
    private readonly List<SpeedSample> _speedSamples = [];

    private TimeSpan? _previousKeystrokeTime;
    private double _nextSampleSecond = SampleIntervalSeconds;
    private int _currentWordIndex;
    private int _currentLetterIndex;
    private int _lettersBeforeCurrentWord;
    private int _correctKeystrokes;
    private int _wrongKeystrokes;
    private int _correctedKeystrokes;

    public TypingTest(IStopwatch stopwatch, IWordGenerator wordGenerator, GameSettings settings)
    {
        ArgumentNullException.ThrowIfNull(stopwatch);
        ArgumentNullException.ThrowIfNull(wordGenerator);
        ArgumentNullException.ThrowIfNull(settings);

        _stopwatch = stopwatch;
        _wordGenerator = wordGenerator;
        _settings = settings;
        _wordOptions = new WordOptions(settings.PunctuationEnabled, settings.NumbersEnabled);
        _timeLimit = settings.Mode == TestMode.Time
            ? TimeSpan.FromSeconds(settings.TimeLimitSeconds)
            : Timeout.InfiniteTimeSpan;

        AddWords(settings.Mode == TestMode.Words ? settings.WordCount : TimeModeBufferSize);
    }

    public event EventHandler? Changed;

    public IReadOnlyList<Word> Words => _words;

    public TestMode Mode => _settings.Mode;

    public GamePhase Phase { get; private set; } = GamePhase.Ready;

    public TimeSpan TimeLimit => _timeLimit;

    public TimeSpan Duration => _stopwatch.Elapsed;

    public int CurrentWordIndex => _currentWordIndex;

    public int CaretIndex => _lettersBeforeCurrentWord + _currentLetterIndex;

    public Word? CurrentWord => _currentWordIndex < _words.Count ? _words[_currentWordIndex] : null;

    public Letter? CurrentLetter =>
        CurrentWord is { } word && _currentLetterIndex < word.Letters.Count
            ? word.Letters[_currentLetterIndex]
            : null;

    public int Wpm => CalculateWpm();

    public double Accuracy
    {
        get
        {
            var totalKeystrokes = _correctKeystrokes + _wrongKeystrokes + _correctedKeystrokes;
            return totalKeystrokes == 0 ? 100 : Math.Round(_correctKeystrokes * 100.0 / totalKeystrokes, 1);
        }
    }

    public double Consistency => CalculateConsistency();

    public void Type(char character)
    {
        if (Phase == GamePhase.Finished)
        {
            return;
        }

        if (char.IsWhiteSpace(character))
        {
            TypeSeparator();
            return;
        }

        var letter = CurrentLetter;

        if (letter is null)
        {
            return;
        }

        Start();

        var isCorrect = char.ToLowerInvariant(character) == char.ToLowerInvariant(letter.Value);

        letter.State = isCorrect ? LetterState.Correct : LetterState.Wrong;
        _currentLetterIndex++;

        if (isCorrect)
        {
            _correctKeystrokes++;
        }
        else
        {
            _wrongKeystrokes++;
        }

        RecordKeystroke();

        if (CompleteIfLastWordFinished())
        {
            return;
        }

        AppendWordsIfNeeded();
        RaiseChanged();
    }

    public void Backspace()
    {
        if (Phase == GamePhase.Finished || CurrentWord is null)
        {
            return;
        }

        if (_currentLetterIndex > 0)
        {
            _currentLetterIndex--;
        }
        else if (_currentWordIndex > 0)
        {
            _currentWordIndex--;
            var previousWord = _words[_currentWordIndex];
            _lettersBeforeCurrentWord -= previousWord.Letters.Count;
            _currentLetterIndex = previousWord.Letters.Count - 1;
        }
        else
        {
            return;
        }

        if (CurrentLetter is { } letter && letter.State != LetterState.Untyped)
        {
            letter.State = LetterState.Untyped;
            _correctedKeystrokes++;
        }

        RaiseChanged();
    }

    public void Tick()
    {
        if (Phase != GamePhase.Running)
        {
            return;
        }

        if (IsTimeLimitReached())
        {
            Finish();
            return;
        }

        if (AppendWordsIfNeeded())
        {
            RaiseChanged();
        }
    }

    public WordProgress GetWordProgress()
    {
        var typed = 0;
        var correct = 0;

        foreach (var word in _words)
        {
            if (!word.IsFullyTyped)
            {
                break;
            }

            typed++;

            if (!word.ContainsErrors)
            {
                correct++;
            }
        }

        return new WordProgress(typed, correct);
    }

    public TestResult ToResult() =>
        new()
        {
            Mode = Mode,
            Wpm = Wpm,
            Accuracy = Accuracy,
            Consistency = Consistency,
            TypedWords = GetWordProgress().Typed,
            CorrectWords = GetWordProgress().Correct,
            DurationSeconds = Math.Round(Duration.TotalSeconds, 2),
            CompletedAt = DateTimeOffset.Now,
            SpeedSamples = [.. _speedSamples]
        };

    private void TypeSeparator()
    {
        Start();

        if (CurrentWord is not { } word || _currentLetterIndex < word.Letters.Count)
        {
            return;
        }

        _lettersBeforeCurrentWord += word.Letters.Count;
        _currentWordIndex++;
        _currentLetterIndex = 0;

        if (_settings.Mode == TestMode.Words && _currentWordIndex >= _words.Count)
        {
            Finish();
            return;
        }

        AppendWordsIfNeeded();
        RaiseChanged();
    }

    private void Start()
    {
        if (Phase != GamePhase.Ready)
        {
            return;
        }

        Phase = GamePhase.Running;
        _stopwatch.Start();
        RaiseChanged();
    }

    private void Finish()
    {
        if (Phase == GamePhase.Finished)
        {
            return;
        }

        Phase = GamePhase.Finished;
        _stopwatch.Stop();
        _speedSamples.Add(new SpeedSample(Duration.TotalSeconds, Wpm));
        RaiseChanged();
    }

    private bool CompleteIfLastWordFinished()
    {
        if (_settings.Mode != TestMode.Words || _currentWordIndex < _words.Count - 1)
        {
            return false;
        }

        if (_currentLetterIndex != _words[_currentWordIndex].Letters.Count)
        {
            return false;
        }

        Finish();
        return true;
    }

    private bool IsTimeLimitReached() =>
        _settings.Mode == TestMode.Time && Duration >= _timeLimit;

    private void AddWords(int count)
    {
        foreach (var text in _wordGenerator.Generate(count, _wordOptions))
        {
            _words.Add(new Word(text));
        }
    }

    private bool AppendWordsIfNeeded()
    {
        if (_settings.Mode != TestMode.Time || _words.Count - _currentWordIndex > TimeModeBufferThreshold)
        {
            return false;
        }

        AddWords(TimeModeAppendSize);
        return true;
    }

    private void RecordKeystroke()
    {
        var now = _stopwatch.Elapsed;

        if (_previousKeystrokeTime is { } previous)
        {
            var seconds = (now - previous).TotalSeconds;

            if (seconds >= MinimumSampleSeconds)
            {
                _keystrokeSpeeds.Add(WordsPerMinutePerCharacter / seconds);
            }
        }

        _previousKeystrokeTime = now;

        while (now.TotalSeconds >= _nextSampleSecond)
        {
            _speedSamples.Add(new SpeedSample(_nextSampleSecond, CalculateWpm()));
            _nextSampleSecond += SampleIntervalSeconds;
        }
    }

    private int CalculateWpm()
    {
        var minutes = _stopwatch.Elapsed.TotalMinutes;

        if (minutes <= 0 || _correctKeystrokes == 0)
        {
            return 0;
        }

        return (int)Math.Round(_correctKeystrokes / 5.0 * MinutesPerMinute / minutes);
    }

    private double CalculateConsistency()
    {
        if (_keystrokeSpeeds.Count < 2)
        {
            return 0;
        }

        var mean = _keystrokeSpeeds.Average();

        if (mean <= 0)
        {
            return 0;
        }

        var variance = _keystrokeSpeeds.Sum(speed => Math.Pow(speed - mean, 2)) / _keystrokeSpeeds.Count;
        var deviation = Math.Sqrt(variance);

        return Math.Round(Math.Clamp((1 - deviation / mean) * 100, 0, 100));
    }

    private void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);
}
