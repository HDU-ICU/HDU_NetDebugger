using System;
using System.Text;
using System.Threading.Tasks;
using HDU_NetDebugger.Models;
using HDU_NetDebugger.Services;

namespace HDU_NetDebugger.Checkers;

public abstract class CheckerBase : IChecker
{
    protected CheckResultBuilder ResultBuilder { get; }

    protected StringBuilder DetailsBuilder { get; }

    private string BuildDetails()
    {
        return DetailsBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
    }

    protected CheckerBase()
    {
        ResultBuilder = new CheckResultBuilder();
        DetailsBuilder = new StringBuilder();
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
    public void Reset()
    {
        ResultBuilder.Reset();
        DetailsBuilder.Clear();
    }
    protected abstract Task ExecuteCoreAsync();

    // 便捷方法
    protected void Pass(string summary) => ResultBuilder.Pass(summary, BuildDetails());
    protected void Fail(string summary) => ResultBuilder.Fail(summary, BuildDetails());
    protected void Warn(string summary) => ResultBuilder.Warn(summary, BuildDetails());
    protected void AddWarning(string message)
        => ResultBuilder.AddWarning(message);
    protected void AddSuggestion(string suggestion) => ResultBuilder.AddSuggestion(suggestion);
}
