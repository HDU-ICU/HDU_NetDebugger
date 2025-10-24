
using System.Threading.Tasks;
using HDU_NetDebugger.Models;

namespace HDU_NetDebugger.Services;

public interface IChecker
{
    string Name { get; }
    Task<CheckResult> ExecuteAsync();
}
