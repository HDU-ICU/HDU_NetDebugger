using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using HDU_NetDebugger.Models;
using HDU_NetDebugger.Services;

namespace HDU_NetDebugger.ViewModels;

public partial class CheckItemViewModel : ViewModelBase
{
    [ObservableProperty]
    public string _name;
    [ObservableProperty]
    public CheckStatus _status;
    [ObservableProperty]
    public string? _checkDetails;

    public IChecker Checker { get; init; }
    public Dictionary<string, Func<Task<bool>>> Conditions { get; set; }
    public CheckItemViewModel(string? name, IChecker checker, Dictionary<string, Func<Task<bool>>> conditions)
    {
        Name = name ?? string.Empty;
        Checker = checker;
        Conditions = conditions;
        CheckDetails = "此检查尚未进行";
    }

    public async Task RunAsync()
    {
        var conditionTasks = Conditions.ToDictionary(kv => kv.Key, kv => kv.Value());
        var allResults = await Task.WhenAll(conditionTasks.Values);
        var firstFailed = conditionTasks.FirstOrDefault(kv => !allResults[Array.IndexOf(conditionTasks.Values.ToArray(), conditionTasks[kv.Key])]);
        if (firstFailed.Key != null)
        {
            Status = CheckStatus.Skipped;
            CheckDetails = $"条件 \"{firstFailed.Key}\" 未满足，跳过此检查";
            return;
        }
    }
}