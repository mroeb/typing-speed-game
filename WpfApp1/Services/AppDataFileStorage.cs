using System.IO;
using WpfApp1.Core.Settings;

namespace WpfApp1.Services;

public sealed class AppDataFileStorage : IFileStorage
{
    private const string ApplicationFolder = "WpfType";

    private readonly string _directory;

    public AppDataFileStorage() : this(DefaultDirectory()) { }

    public AppDataFileStorage(string directory) => _directory = directory;

    public string Read(string fileName) => File.ReadAllText(Resolve(fileName));

    public void Write(string fileName, string content)
    {
        Directory.CreateDirectory(_directory);
        File.WriteAllText(Resolve(fileName), content);
    }

    private string Resolve(string fileName) => Path.Combine(_directory, fileName);

    private static string DefaultDirectory() =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationFolder);
}
