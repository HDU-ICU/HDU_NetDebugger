
using System.Threading.Tasks;

namespace HDU_NetDebugger.Models;

public interface IChecker
{
    Task<CheckResult> ExecuteAsync();
    void Reset();
}
