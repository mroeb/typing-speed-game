using WpfApp1.Core.Results;
using WpfApp1.Core.Settings;
using WpfApp1.Core.Text;
using WpfApp1.Services;
using WpfApp1.Themes;
using WpfApp1.ViewModels;

namespace WpfApp1;

public static class CompositionRoot
{
    public static GameViewModel CreateViewModel()
    {
        var fileStorage = new AppDataFileStorage();

        return new GameViewModel(
            new SystemStopwatch(),
            new DispatcherTickSource(),
            new RandomWordGenerator(CommonVocabulary.Words),
            new JsonGameSettingsRepository(fileStorage),
            new JsonResultRepository(fileStorage),
            new ThemeService());
    }
}
