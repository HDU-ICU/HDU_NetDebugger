using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace HDU_NetDebugger.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public ObservableCollection<CheckItemViewModel> CheckItems { get; } = [];
}
