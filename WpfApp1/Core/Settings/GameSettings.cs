using WpfApp1.Core.Enums;

namespace WpfApp1.Core.Settings;

public sealed class GameSettings
{
    public TestMode Mode { get; set; } = TestMode.Words;

    public int WordCount { get; set; } = 25;

    public int TimeLimitSeconds { get; set; } = 30;

    public bool PunctuationEnabled { get; set; }

    public bool NumbersEnabled { get; set; }

    public string ThemeName { get; set; } = "dark";
}
