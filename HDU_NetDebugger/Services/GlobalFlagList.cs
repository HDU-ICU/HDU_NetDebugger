using System.Collections.Concurrent;
namespace HDU_NetDebugger.Services;

public class GlobalFlagList
{
    public static ConcurrentDictionary<string, object> FlagList { get; set; } = [];
}