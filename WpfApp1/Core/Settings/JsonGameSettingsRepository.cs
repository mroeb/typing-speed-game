using System.IO;
using System.Text.Json;

namespace WpfApp1.Core.Settings;

public sealed class JsonGameSettingsRepository : IGameSettingsRepository
{
    private const string FileName = "settings.json";

    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    private readonly IFileStorage _storage;

    public JsonGameSettingsRepository(IFileStorage storage) => _storage = storage;

    public GameSettings Load()
    {
        try
        {
            return JsonSerializer.Deserialize<GameSettings>(_storage.Read(FileName)) ?? new GameSettings();
        }
        catch (JsonException)
        {
            return new GameSettings();
        }
        catch (IOException)
        {
            return new GameSettings();
        }
    }

    public void Save(GameSettings settings) =>
        _storage.Write(FileName, JsonSerializer.Serialize(settings, SerializerOptions));
}
