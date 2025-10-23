using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace HDU_NetDebugger.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public ObservableCollection<CheckItemViewModel> CheckItems { get; } = [
        new CheckItemViewModel("网关连通性"),
        new CheckItemViewModel("DNS"),
        new CheckItemViewModel("认证状态"),
        new CheckItemViewModel("互联网连接"),
    ];
}
