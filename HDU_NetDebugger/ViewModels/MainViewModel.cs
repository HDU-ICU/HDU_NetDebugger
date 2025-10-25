using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HDU_NetDebugger.Services;
using HDU_NetDebugger.Checkers;
using System;
using System.Collections.ObjectModel;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace HDU_NetDebugger.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        foreach (var (name, checker, conditions) in CheckServices.GetAvailableCheckers([
            "HDU_NetDebugger.Checkers",
            "HDU_NetDebugger.Desktop.Checkers",
            "HDU_NetDebugger.Android.Checkers",
            "HDU_NetDebugger.iOS.Checkers",
            "HDU_NetDebugger.Browser.Checkers"
        ]))
        {
            if (checker is not null)
            {
                CheckItems.Add(new CheckItemViewModel(name, checker, conditions));
            }
        }
        RunChecksCommand.NotifyCanExecuteChanged();
        Console.WriteLine($"Loaded {CheckItems.Count} checkers.");
    }
    public ObservableCollection<CheckItemViewModel> CheckItems { get; } = [];

    [RelayCommand(CanExecute = nameof(CanRunChecks))]
    public async Task RunChecks()
    {
        Console.WriteLine("Running checks...");
        IsRunningChecks = true;
        foreach (var item in CheckItems)
            await item.RunAsync();
        IsRunningChecks = false;
    }
    public bool CanRunChecks() => HasItems && !IsRunningChecks;
    private bool HasItems => CheckItems?.Count > 0;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunChecksCommand))]
    public bool isRunningChecks = false;

}
