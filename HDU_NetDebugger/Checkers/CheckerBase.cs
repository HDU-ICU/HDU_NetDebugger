using System;
using System.Threading.Tasks;
using HDU_NetDebugger.Models;
using HDU_NetDebugger.Services;

namespace HDU_NetDebugger.Checkers;

public abstract class CheckerBase : IChecker
{
    protected CheckResultBuilder ResultBuilder { get; }

    protected CheckerBase()
    {
        ResultBuilder = new CheckResultBuilder();
    }

    public async Task<CheckResult> ExecuteAsync()
    {
        try
        {
            await ExecuteCoreAsync();
            return ResultBuilder.Build();
        }
        catch (Exception ex)
        {
            return ResultBuilder
                .Fail($"检查执行失败: {ex.Message}", $"异常详情: {ex}")
                .Build();
        }
    }

    protected abstract Task ExecuteCoreAsync();

    // 便捷方法
    protected void Pass(string summary, string? details = null) => ResultBuilder.Pass(summary, details);
    protected void Fail(string summary, string? details = null) => ResultBuilder.Fail(summary, details);
    protected void Warn(string summary, string? details = null) => ResultBuilder.Warn(summary, details);
    protected void AddWarning(string message)
        => ResultBuilder.AddWarning(message);
    protected void AddSuggestion(string suggestion) => ResultBuilder.AddSuggestion(suggestion);
}
