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

    public async Task<CheckResult> ExecuteAsync()
    {
        await Task.Delay(1000); // Simulate some work
        return new CheckResult
        {
            IsSuccessful = true,
            Details = "Test Checker executed successfully."
        };
    }
}

[Checker("Test Checker 2", 2)]
[CheckerCondition("Always False Condition", nameof(AlwaysFalseCondition))]
public class TestChecker2 : IChecker
{
    public static Task<bool> AlwaysFalseCondition() => Task.FromResult(false);

    public async Task<CheckResult> ExecuteAsync()
    {
        await Task.Delay(1000); // Simulate some work
        return new CheckResult
        {
            IsSuccessful = true,
            Details = "Test Checker executed successfully."
        };
    }
}

[Checker("Test Checker 3", 3)]
public class TestChecker3 : IChecker
{
    public async Task<CheckResult> ExecuteAsync()
    {
        await Task.Delay(5000); // Simulate some work
        return new CheckResult
        {
            IsSuccessful = false,
            Details = "Test Checker executed failed."
        };
    }
}

[Checker("Test Checker 4", 4)]
public class TestChecker4 : IChecker
{
    public async Task<CheckResult> ExecuteAsync()
    {
        await Task.Delay(5000); // Simulate some work
        return new CheckResult
        {
            IsSuccessful = false,
            Details = "Test Checker executed failed."
        };
    }
}

[Checker("Test Checker 5", 5)]
public class TestChecker5 : IChecker
{
    public async Task<CheckResult> ExecuteAsync()
    {
        await Task.Delay(1000); // Simulate some work
        GlobalFlagList.FlagList["TestFlag"] = "TestValue";
        return new CheckResult
        {
            IsSuccessful = false,
            Details = "Test Checker executed failed."
        };
    }
}

[Checker("Test Checker 6", 6)]
[CheckerCondition("Is Test Flag Set", nameof(IsTestFlagSet))]
public class TestChecker6 : IChecker
{
    public static Task<bool> IsTestFlagSet() => Task.FromResult(GlobalFlagList.FlagList.ContainsKey("TestFlag"));
    public async Task<CheckResult> ExecuteAsync()
    {
        await Task.Delay(1000); // Simulate some work
        return new CheckResult
        {
            IsSuccessful = true,
            Details = "Test Checker executed successfully."
        };
    }
}