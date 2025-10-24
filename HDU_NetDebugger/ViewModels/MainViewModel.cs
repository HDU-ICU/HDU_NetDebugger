using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HDU_NetDebugger.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HDU_NetDebugger.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        foreach (var (checker, conditions) in CheckServices.GetAvailableCheckers(["HDU_NetDebugger.Checkers.Network"]))
        {
            if (checker is not null)
            {
                CheckItems.Add(new CheckItemViewModel(checker.Name, checker, conditions));
            }
        }
    }
    public ObservableCollection<CheckItemViewModel> CheckItems { get; } = [];
}
