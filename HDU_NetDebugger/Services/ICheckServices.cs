using System.Collections.Generic;
using System.Threading.Tasks;
using HDU_NetDebugger.Models;

namespace HDU_NetDebugger.Services;

public interface ICheckServices
{
    Task<IEnumerable<CheckResult>> RunAllChecksAsync();
    Task<CheckResult> RunCheckAsync(string checkName);
    IEnumerable<IChecker> GetAvailableCheckers();
}
