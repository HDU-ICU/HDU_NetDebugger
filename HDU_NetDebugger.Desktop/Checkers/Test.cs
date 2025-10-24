using System.Threading;
using System.Threading.Tasks;
using HDU_NetDebugger.Attributes;
using HDU_NetDebugger.Models;
using HDU_NetDebugger.Services;

namespace HDU_NetDebugger.Checkers;

[Checker("Test Checker", 1)]
[CheckerCondition("Always True Condition", nameof(AlwaysTrueCondition))]
public class TestChecker : IChecker
{
    public static Task<bool> AlwaysTrueCondition() => Task.FromResult(true);

    public Task<CheckResult> ExecuteAsync()
    {
        Thread.Sleep(1000); // Simulate some work
        return Task.FromResult(new CheckResult
        {
            IsSuccessful = true,
            Details = "Test Checker executed successfully."
        });
    }
}