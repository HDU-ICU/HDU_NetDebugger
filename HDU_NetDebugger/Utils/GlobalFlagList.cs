using System.Collections.Concurrent;
namespace HDU_NetDebugger.Utils;

public class GlobalFlagList
{
    public static ConcurrentDictionary<string, object> FlagList { get; set; } = [];
}