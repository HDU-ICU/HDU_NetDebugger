using System.Net;

namespace HDU_NetDebugger.Models;

public static class HDUConst
{
    public static readonly IPNetwork DormNetwork = IPNetwork.Parse("10.150.0.0/16");    // 生活区网段
    public static readonly IPNetwork CampusNetwork = IPNetwork.Parse("10.249.0.0/16");  // 校园网段
    public static readonly IPAddress BASIp = IPAddress.Parse("10.150.0.1");             // BAS
    public static readonly IPAddress SrunIp = IPAddress.Parse("192.168.112.97");        // 深澜认证服务器
}