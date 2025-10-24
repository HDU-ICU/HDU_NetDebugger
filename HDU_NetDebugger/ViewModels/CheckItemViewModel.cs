using CommunityToolkit.Mvvm.ComponentModel;
using HDU_NetDebugger.Models;

namespace HDU_NetDebugger.ViewModels;

public partial class CheckItemViewModel : ViewModelBase
{
    [ObservableProperty]
    public string _name;
    [ObservableProperty]
    public CheckStatus _status;
    [ObservableProperty]
    public string? _checkDetails;

    public CheckItemViewModel(string name, CheckStatus status = CheckStatus.UnChecked)
    {
        Name = name;
        Status = status;
        CheckDetails = "此检查尚未进行";
    }
}