using System.IO;
using System.Text.Json;
using WpfApp1.Core.Enums;
using WpfApp1.Core.Settings;

namespace WpfApp1.Core.Results;

public sealed class JsonResultRepository : IResultRepository
{
    private const string FileName = "results.json";
    private const int MaxStoredResults = 100;

    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    private readonly IFileStorage _storage;

    public JsonResultRepository(IFileStorage storage) => _storage = storage;

    public IReadOnlyList<TestResult> GetResults()
    {
        try
        {
            return JsonSerializer.Deserialize<List<TestResult>>(_storage.Read(FileName)) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
        catch (IOException)
        {
            return [];
        }
    }

    public TestResult? GetBest(TestMode mode)
    {
        TestResult? best = null;

        foreach (var result in GetResults())
        {
            if (result.Mode != mode)
            {
                continue;
            }

            if (best is null || result.Wpm > best.Wpm)
            {
                best = result;
            }
        }

        return best;
    }

    public void Save(TestResult result)
    {
        var results = GetResults().ToList();
        results.Add(result);

        if (results.Count > MaxStoredResults)
        {
            results = results[^MaxStoredResults..];
        }

        _storage.Write(FileName, JsonSerializer.Serialize(results, SerializerOptions));
    }
}
