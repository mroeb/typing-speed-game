namespace WpfApp1.Core.Settings;

public interface IGameSettingsRepository
{
    GameSettings Load();

    void Save(GameSettings settings);
}
