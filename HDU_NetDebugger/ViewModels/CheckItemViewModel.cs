
namespace HDU_NetDebugger.ViewModels;

public partial class CheckItemViewModel : ViewModelBase
{

    public enum CheckStatus
    {
        UnChecked,
        Checking,
        CheckedNoError,
        CheckedWithError
    }
    public string Name { get; set; }
    public CheckStatus Status { get; set; }
    public string? CheckDetails { get; set; }

    public CheckItemViewModel(string name, CheckStatus status = CheckStatus.UnChecked)
    {
        Name = name;
        Status = status;
        CheckDetails = string.Empty;
    }
}