
using System.Threading.Tasks;
using HDU_NetDebugger.Models;

namespace HDU_NetDebugger.Services;

public interface IChecker
{
    Task<CheckResult> ExecuteAsync();
}
