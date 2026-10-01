using WpfApp1.Core.Enums;

namespace WpfApp1.Core.Results;

public interface IResultRepository
{
    IReadOnlyList<TestResult> GetResults();

    TestResult? GetBest(TestMode mode);

    void Save(TestResult result);
}
