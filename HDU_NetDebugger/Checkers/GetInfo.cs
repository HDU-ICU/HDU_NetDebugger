using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using HDU_NetDebugger.Attributes;
using HDU_NetDebugger.Models;
using HDU_NetDebugger.Services;

namespace HDU_NetDebugger.Checkers;

[Checker("获取基本信息", -1)]
public class GetInfo : CheckerBase
{
    protected override async Task ExecuteCoreAsync()
    {
        var detailsBuilder = new StringBuilder();
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        if (networkInterfaces.Length == 0)
        {
            Fail("未获取到网络接口信息", string.Empty);
            return;
        }
        foreach (var ni in networkInterfaces)
        {
            if (ni.OperationalStatus == OperationalStatus.Up &&
                ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                HasIPv4Address(ni))
            {
                detailsBuilder.AppendLine(BuildIFInfo(ni));
                if (HasDormIPv4Address(ni))
                {
                    GlobalFlagList.FlagList["NetWorkType"] = "Dorm";
                }
                else if (HasCampusIPv4Address(ni))
                {
                    GlobalFlagList.FlagList["NetWorkType"] = "Campus";
                }
            }
        }
        if (detailsBuilder.Length == 0)
        {
            Fail("未找到符合条件的网络接口", "未找到符合条件的网络接口（以太网且已启用且有IPv4地址）\n");
        }
        else
        {
            Pass(GetSummary(), detailsBuilder.ToString().TrimEnd('\n'));
        }
    }
    private static bool HasIPv4Address(NetworkInterface ni)
    {
        var ipProps = ni.GetIPProperties();
        return ipProps.UnicastAddresses
            .Any(addr => addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
    }
    private string GetSummary()
    {
        if (GlobalFlagList.FlagList.TryGetValue("NetWorkType", out var netType))
        {
            string netTypeStr = netType as string ?? string.Empty;
            if (netTypeStr == "Dorm")
            {
                return "检测到生活区网络";
            }
            else if (netTypeStr == "Campus")
            {
                return "检测到教学区网络";
            }
        }
        AddWarning("未检测到校园网地址，如有路由器请忽略");
        return "未检测到校园网地址，如有路由器请忽略";
    }
    private static bool HasDormIPv4Address(NetworkInterface ni)
    {
        return ni.GetIPProperties().UnicastAddresses
            .Any(addr => HDUConst.DormNetwork.Contains(addr.Address));
    }
    private static bool HasCampusIPv4Address(NetworkInterface ni)
    {
        return ni.GetIPProperties().UnicastAddresses
            .Any(addr => HDUConst.CampusNetwork.Contains(addr.Address));
    }
    private string BuildIFInfo(NetworkInterface ni)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"接口名称: {ni.Name}");
        sb.AppendLine($"描述: {ni.Description}");
        sb.AppendLine($"类型: {ni.NetworkInterfaceType}");
        sb.AppendLine($"状态: {ni.OperationalStatus}");
        var ipProps = ni.GetIPProperties();
        var ipv4Addrs = ipProps.UnicastAddresses
            .Where(addr => addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .Select(addr => addr.Address.ToString());
        sb.AppendLine($"IPv4 地址: {string.Join(", ", ipv4Addrs)}");
        var ipv6Addrs = ipProps.UnicastAddresses
            .Where(addr => addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            .Select(addr => addr.Address.ToString());
        sb.AppendLine($"IPv6 地址: {string.Join(", ", ipv6Addrs)}");
        return sb.ToString();
    }
}