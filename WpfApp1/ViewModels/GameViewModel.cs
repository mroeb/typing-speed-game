using System.Collections.ObjectModel;
using WpfApp1.Core.Enums;
using WpfApp1.Core.Results;
using WpfApp1.Core.Settings;
using WpfApp1.Core.Text;
using WpfApp1.Core.Time;
using WpfApp1.Core.Typing;
using WpfApp1.Themes;

namespace WpfApp1.ViewModels;

public sealed class GameViewModel : ViewModelBase
{
    private const int BlinkIntervalTicks = 6;

    private static readonly int[] AvailableTimeLimits = [15, 30, 60, 120];

    private static readonly int[] AvailableWordCounts = [10, 25, 50, 100];

    private readonly IStopwatch _stopwatch;
    private readonly ITickSource _tickSource;
    private readonly IWordGenerator _wordGenerator;
    private readonly IGameSettingsRepository _settingsRepository;
    private readonly IResultRepository _resultRepository;
    private readonly IThemeService _themeService;
    private readonly ObservableCollection<WordViewModel> _words = [];
    private readonly List<LetterViewModel> _letters = [];

    private GameSettings _settings = new();
    private TypingTest _test = null!;
    private LetterViewModel? _currentLetter;
    private WordViewModel? _currentWord;
    private ResultViewModel _result = new();
    private int _previousCaretIndex;
    private int _tickCount;
    private bool _isCaretVisible = true;
    private int _wpm;
    private int _bestWpm;
    private int _typedWords;
    private double _accuracy;
    private double _progressValue;
    private string _progressText = string.Empty;
    private bool _isTimeMode;
    private bool _isWordsMode;
    private bool _isPunctuationEnabled;
    private bool _isNumbersEnabled;
    private bool _isResultVisible;

    public GameViewModel(
        IStopwatch stopwatch,
        ITickSource tickSource,
        IWordGenerator wordGenerator,
        IGameSettingsRepository settingsRepository,
        IResultRepository resultRepository,
        IThemeService themeService)
    {
        ArgumentNullException.ThrowIfNull(stopwatch);
        ArgumentNullException.ThrowIfNull(tickSource);
        ArgumentNullException.ThrowIfNull(wordGenerator);
        ArgumentNullException.ThrowIfNull(settingsRepository);
        ArgumentNullException.ThrowIfNull(resultRepository);
        ArgumentNullException.ThrowIfNull(themeService);

        _stopwatch = stopwatch;
        _tickSource = tickSource;
        _wordGenerator = wordGenerator;
        _settingsRepository = settingsRepository;
        _resultRepository = resultRepository;
        _themeService = themeService;

        Words = new ReadOnlyObservableCollection<WordViewModel>(_words);
        Modes = [.. Enum.GetValues<TestMode>().Select(mode => new OptionViewModel<TestMode>(mode, mode.ToString().ToLowerInvariant()))];
        TimeOptions = [.. AvailableTimeLimits.Select(seconds => new OptionViewModel<int>(seconds, seconds.ToString()))];
        WordCountOptions = [.. AvailableWordCounts.Select(count => new OptionViewModel<int>(count, count.ToString()))];
        Themes = [.. ThemeCatalog.All.Select(theme => new OptionViewModel<Theme>(theme, theme.Name))];

        RestartCommand = new RelayCommand(Restart);
        SelectModeCommand = new RelayCommand<OptionViewModel<TestMode>>(SelectMode);
        SelectTimeCommand = new RelayCommand<OptionViewModel<int>>(SelectTime);
        SelectWordCountCommand = new RelayCommand<OptionViewModel<int>>(SelectWordCount);
        SelectThemeCommand = new RelayCommand<OptionViewModel<Theme>>(SelectTheme);

        _tickSource.Tick += OnTick;
        _settings = _settingsRepository.Load();
        ApplySettings();
        _themeService.Apply(ThemeCatalog.Get(_settings.ThemeName));
        _bestWpm = _resultRepository.GetBest(_settings.Mode)?.Wpm ?? 0;
        CreateTest();
    }

    public IReadOnlyList<WordViewModel> Words { get; }

    public IReadOnlyList<OptionViewModel<TestMode>> Modes { get; }

    public IReadOnlyList<OptionViewModel<int>> TimeOptions { get; }

    public IReadOnlyList<OptionViewModel<int>> WordCountOptions { get; }

    public IReadOnlyList<OptionViewModel<Theme>> Themes { get; }

    public RelayCommand RestartCommand { get; }

    public RelayCommand<OptionViewModel<TestMode>> SelectModeCommand { get; }

    public RelayCommand<OptionViewModel<int>> SelectTimeCommand { get; }

    public RelayCommand<OptionViewModel<int>> SelectWordCountCommand { get; }

    public RelayCommand<OptionViewModel<Theme>> SelectThemeCommand { get; }

    public int Wpm
    {
        get => _wpm;
        private set => SetProperty(ref _wpm, value);
    }

    public double Accuracy
    {
        get => _accuracy;
        private set => SetProperty(ref _accuracy, value);
    }

    public int TypedWords
    {
        get => _typedWords;
        private set => SetProperty(ref _typedWords, value);
    }

    public int BestWpm
    {
        get => _bestWpm;
        private set => SetProperty(ref _bestWpm, value);
    }

    public string ProgressText
    {
        get => _progressText;
        private set => SetProperty(ref _progressText, value);
    }

    public double ProgressValue
    {
        get => _progressValue;
        private set => SetProperty(ref _progressValue, value);
    }

    public bool IsResultVisible
    {
        get => _isResultVisible;
        private set => SetProperty(ref _isResultVisible, value);
    }

    public bool IsTimeMode
    {
        get => _isTimeMode;
        private set => SetProperty(ref _isTimeMode, value);
    }

    public bool IsWordsMode
    {
        get => _isWordsMode;
        private set => SetProperty(ref _isWordsMode, value);
    }

    public bool IsPunctuationEnabled
    {
        get => _isPunctuationEnabled;
        set
        {
            if (!SetProperty(ref _isPunctuationEnabled, value) || value == _settings.PunctuationEnabled)
            {
                return;
            }

            ChangeSettings(settings => settings.PunctuationEnabled = value);
        }
    }

    public bool IsNumbersEnabled
    {
        get => _isNumbersEnabled;
        set
        {
            if (!SetProperty(ref _isNumbersEnabled, value) || value == _settings.NumbersEnabled)
            {
                return;
            }

            ChangeSettings(settings => settings.NumbersEnabled = value);
        }
    }

    public ResultViewModel Result
    {
        get => _result;
        private set => SetProperty(ref _result, value);
    }

    public void TypeCharacter(char character)
    {
        if (IsResultVisible)
        {
            return;
        }

        if (_test.Phase == GamePhase.Ready)
        {
            _tickSource.Start();
        }

        _test.Type(character);
    }

    public void Backspace()
    {
        if (IsResultVisible)
        {
            return;
        }

        _test.Backspace();
    }

    public void Restart()
    {
        _tickSource.Stop();
        _test.Changed -= OnTestChanged;
        IsResultVisible = false;
        Result = new ResultViewModel();
        CreateTest();
    }

    private void CreateTest()
    {
        _stopwatch.Reset();
        _test = new TypingTest(_stopwatch, _wordGenerator, _settings);
        _test.Changed += OnTestChanged;
        _words.Clear();
        _letters.Clear();
        BuildWordViewModels();
        _previousCaretIndex = 0;
        _tickCount = 0;
        _isCaretVisible = true;
        UpdateCaret();
        RefreshLetters();
        RefreshStatistics();
    }

    private void OnTestChanged(object? sender, EventArgs e)
    {
        BuildWordViewModels();
        RefreshLetters();
        UpdateCaret();
        RefreshStatistics();

        if (_test.Phase == GamePhase.Finished)
        {
            Complete();
        }
    }

    private void OnTick(object? sender, EventArgs e)
    {
        _test.Tick();
        RefreshStatistics();
        ToggleCaretBlink();
    }

    private void ToggleCaretBlink()
    {
        if (_currentLetter is null || ++_tickCount % BlinkIntervalTicks != 0)
        {
            return;
        }

        _currentLetter.IsCaretVisible = _isCaretVisible = !_isCaretVisible;
    }

    private void BuildWordViewModels()
    {
        for (var index = _words.Count; index < _test.Words.Count; index++)
        {
            var wordViewModel = new WordViewModel(_test.Words[index]);

            _words.Add(wordViewModel);
            _letters.AddRange(wordViewModel.Letters);
        }
    }

    private void UpdateCaret()
    {
        if (_currentLetter is not null)
        {
            _currentLetter.IsCurrent = false;
        }

        if (_currentWord is not null)
        {
            _currentWord.IsAwaitingSeparator = false;
        }

        _currentWord = _test.CurrentWordIndex < _words.Count ? _words[_test.CurrentWordIndex] : null;
        _currentLetter = _test.CaretIndex < _letters.Count ? _letters[_test.CaretIndex] : null;

        if (_currentWord is not null)
        {
            _currentWord.IsAwaitingSeparator = _currentLetter is null;
        }

        if (_currentLetter is not null)
        {
            _currentLetter.IsCurrent = true;
        }
    }

    private void RefreshLetters()
    {
        if (_previousCaretIndex < _letters.Count)
        {
            _letters[_previousCaretIndex].RefreshState();
        }

        if (_test.CaretIndex < _letters.Count)
        {
            _letters[_test.CaretIndex].RefreshState();
        }

        _previousCaretIndex = _test.CaretIndex;
    }

    private void RefreshStatistics()
    {
        var progress = _test.GetWordProgress();

        Wpm = _test.Wpm;
        Accuracy = _test.Accuracy;
        TypedWords = progress.Typed;
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        if (_isTimeMode)
        {
            var remaining = _test.TimeLimit - _test.Duration;

            ProgressValue = _test.TimeLimit.TotalSeconds <= 0
                ? 0
                : Math.Clamp(remaining.TotalSeconds / _test.TimeLimit.TotalSeconds, 0, 1);
            ProgressText = ResultViewModel.FormatDuration(remaining < TimeSpan.Zero ? TimeSpan.Zero : remaining);
            return;
        }

        ProgressValue = _settings.WordCount <= 0 ? 0 : (double)TypedWords / _settings.WordCount;
        ProgressText = $"{TypedWords} / {_settings.WordCount}";
    }

    private void Complete()
    {
        _tickSource.Stop();
        _stopwatch.Stop();

        var previousBest = _resultRepository.GetBest(_test.Mode);
        var result = _test.ToResult();
        _resultRepository.Save(result);

        BestWpm = Math.Max(BestWpm, result.Wpm);
        Result = new ResultViewModel(result, result.Wpm > (previousBest?.Wpm ?? 0));
        IsResultVisible = true;
    }

    private void ApplySettings()
    {
        IsTimeMode = _settings.Mode == TestMode.Time;
        IsWordsMode = _settings.Mode == TestMode.Words;
        IsPunctuationEnabled = _settings.PunctuationEnabled;
        IsNumbersEnabled = _settings.NumbersEnabled;

        SelectOption(Modes, _settings.Mode);
        SelectOption(TimeOptions, _settings.TimeLimitSeconds);
        SelectOption(WordCountOptions, _settings.WordCount);
        SelectOption(Themes, ThemeCatalog.Get(_settings.ThemeName));
    }

    private void ChangeSettings(Action<GameSettings> change, bool restartTest = true)
    {
        change(_settings);
        _settingsRepository.Save(_settings);

        if (restartTest)
        {
            Restart();
        }
    }

    private void SelectMode(OptionViewModel<TestMode>? option)
    {
        if (option is null || _settings.Mode == option.Value)
        {
            return;
        }

        SelectOption(Modes, option.Value);
        ChangeSettings(settings => settings.Mode = option.Value);
    }

    private void SelectTime(OptionViewModel<int>? option)
    {
        if (option is null || _settings.TimeLimitSeconds == option.Value)
        {
            return;
        }

        SelectOption(TimeOptions, option.Value);
        ChangeSettings(settings => settings.TimeLimitSeconds = option.Value);
    }

    private void SelectWordCount(OptionViewModel<int>? option)
    {
        if (option is null || _settings.WordCount == option.Value)
        {
            return;
        }

        SelectOption(WordCountOptions, option.Value);
        ChangeSettings(settings => settings.WordCount = option.Value);
    }

    private void SelectTheme(OptionViewModel<Theme>? option)
    {
        if (option is null)
        {
            return;
        }

        SelectOption(Themes, option.Value);
        _themeService.Apply(option.Value);
        ChangeSettings(settings => settings.ThemeName = option.Value.Name, restartTest: false);
    }

    private static void SelectOption<T>(IEnumerable<OptionViewModel<T>> options, T value)
    {
        foreach (var option in options)
        {
            option.IsSelected = EqualityComparer<T>.Default.Equals(option.Value, value);
        }
    }
}
