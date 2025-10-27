using System.Net;

namespace HDU_NetDebugger.Models;

public static class HDUConst
{
    // 生活区网段
    public static readonly IPNetwork DormNetwork = IPNetwork.Parse("10.150.0.0/16");
    // 校园网段
    public static readonly IPNetwork CampusNetwork = IPNetwork.Parse("10.249.0.0/16");
}