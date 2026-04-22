using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HDU_NetDebugger.Services;
using HDU_NetDebugger.Utils;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HDU_NetDebugger.Models;

namespace HDU_NetDebugger.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        foreach (var info in CheckerCollector.GetRegistered())
        {
            if (Activator.CreateInstance(info.CheckerType) is IChecker checker)
            {
                CheckItems.Add(new CheckItemViewModel(info.Name, checker, info.Conditions));
            }
        }
        RunChecksCommand.NotifyCanExecuteChanged();
        Console.WriteLine($"Loaded {CheckItems.Count} checkers.");
    }
    public ObservableCollection<CheckItemViewModel> CheckItems { get; } = [];

    [RelayCommand(CanExecute = nameof(CanRunChecks))]
    public async Task RunChecks()
    {
        if (IsChecksRanOnce)
        {
            foreach (var item in CheckItems)
                item.Reset();
        }
        Console.WriteLine("Running checks...");
        IsRunningChecks = true;
        LastCheckTime = DateTime.Now;
        foreach (var item in CheckItems)
            await item.RunAsync();
        IsRunningChecks = false;
        IsChecksRanOnce = true;
    }
    public bool CanRunChecks() => HasItems && !IsRunningChecks;
    private bool HasItems => CheckItems?.Count > 0;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunChecksCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExportResultsCommand))]
    public bool isRunningChecks = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExportResultsCommand))]
    public bool isChecksRanOnce = false;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExportResultsCommand))]
    public bool isExportingResults = false;

    public bool CanExportResults => IsChecksRanOnce && !IsExportingResults && !IsRunningChecks;

    private DateTime LastCheckTime { get; set; } = DateTime.MinValue;

    [RelayCommand(CanExecute = nameof(CanExportResults))]
    public async Task ExportResults()
    {
        IsExportingResults = true;
        await ResultExportUtils.ExportAsync(CheckItems, LastCheckTime);
        IsExportingResults = false;
    }
}
