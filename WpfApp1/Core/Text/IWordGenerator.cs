namespace WpfApp1.Core.Text;

public interface IWordGenerator
{
    IReadOnlyList<string> Generate(int count, WordOptions options);
}
