using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using HDU_NetDebugger.Models;

namespace HDU_NetDebugger.ViewModels;

public partial class CheckItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private CheckStatus _status;
    [ObservableProperty]
    private string? _checkSummary;
    [ObservableProperty]
    private string? _checkDetails;
    [ObservableProperty]
    private ObservableCollection<string> _warnings = [];
    [ObservableProperty]
    private ObservableCollection<string> _suggestions = [];
    [ObservableProperty]
    private bool _isExpanded = false;

    public IChecker Checker { get; init; }
    public Dictionary<string, Func<Task<bool>>> Conditions { get; set; }
    public CheckItemViewModel(string? name, IChecker checker, Dictionary<string, Func<Task<bool>>> conditions)
    {
        Name = name ?? string.Empty;
        Checker = checker;
        Conditions = conditions;
        Reset();
    }

    public void Reset()
    {
        Status = CheckStatus.UnChecked;
        CheckSummary = string.Empty;
        CheckDetails = "此检查尚未进行";
        Warnings.Clear();
        Suggestions.Clear();
        Checker.Reset();
    }

    public async Task RunAsync()
    {
        Console.WriteLine($"Running check: {Name}");
        var conditionTasks = Conditions.ToDictionary(kv => kv.Key, kv => kv.Value());
        var allResults = await Task.WhenAll(conditionTasks.Values);
        var firstFailed = conditionTasks.FirstOrDefault(kv => !allResults[Array.IndexOf(conditionTasks.Values.ToArray(), conditionTasks[kv.Key])]);
        if (firstFailed.Key != null)
        {
            Status = CheckStatus.Skipped;
            CheckSummary = "检查被跳过";
            CheckDetails = $"条件 \"{firstFailed.Key}\" 未满足，跳过此检查";
            return;
        }
        try
        {
            Status = CheckStatus.Checking;
            var result = await Checker.ExecuteAsync();
            CheckStatus status = result.Status switch
            {
                CheckResultStatus.Pass => CheckStatus.Passed,
                CheckResultStatus.Fail => CheckStatus.Failed,
                CheckResultStatus.Warn => CheckStatus.Warned,
                _ => CheckStatus.Failed
            };
            Status = status;
            CheckDetails = result.Details;
            CheckSummary = result.Summary;
            if (UpdateObservableCollection(Warnings, result.Warnings))
            {
                OnPropertyChanged(nameof(Warnings));
            }
            if (UpdateObservableCollection(Suggestions, result.Suggestions))
            {
                OnPropertyChanged(nameof(Suggestions));
            }

            // 自动展开失败或警告的检查项
            if (status == CheckStatus.Failed || status == CheckStatus.Warned)
            {
                IsExpanded = true;
            }
        }
        catch (Exception ex)
        {
            Status = CheckStatus.Failed;
            CheckSummary = "检查执行失败";
            CheckDetails = $"检查执行时发生异常: {ex.Message}";
        }
    }
    private static bool UpdateObservableCollection(ObservableCollection<string> collection, List<string> newItems)
    {
        Console.WriteLine($"UpdateObservableCollection on Thread ID: {Thread.CurrentThread.ManagedThreadId}");
        bool changed = false;
        if (collection.Count != newItems.Count || !collection.SequenceEqual(newItems))
        {
            collection.Clear();
            foreach (var item in newItems)
            {
                collection.Add(item);
            }
            changed = true;
        }
        return changed;
    }
}