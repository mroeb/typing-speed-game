namespace WpfApp1.Core.Settings;

public interface IFileStorage
{
    string Read(string fileName);

    void Write(string fileName, string content);
}
