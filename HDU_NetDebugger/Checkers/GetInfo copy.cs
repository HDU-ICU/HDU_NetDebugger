using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using HDU_NetDebugger.Attributes;
using HDU_NetDebugger.Models;

namespace HDU_NetDebugger.Checkers;

[Checker("test", -2)]
public class Test : IChecker
{
    async Task<CheckResult> IChecker.ExecuteAsync()
    {
        await Task.Delay(1000);
        return new CheckResult
        {
            Status = CheckResultStatus.Pass,
            Summary = "sadasdsadasd",
            Details = "asdasdsadsada"
        };
    }
}