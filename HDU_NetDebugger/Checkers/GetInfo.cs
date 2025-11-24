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
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        if (networkInterfaces.Length == 0)
        {
            Fail("未获取到网络接口信息");
            return;
        }
        foreach (var ni in networkInterfaces)
        {
            if (ni.OperationalStatus == OperationalStatus.Up &&
                ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                HasIPv4Address(ni))
            {
                DetailsBuilder.Append(BuildIFInfo(ni));
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
        if (DetailsBuilder.Length == 0)
        {
            AddWarning("未找到符合条件的网络接口（以太网且已启用且有IPv4地址）");
            Fail("未找到符合条件的网络接口");
        }
        else
        {
            Pass(GetSummary());
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
        AddWarning("未检测到校园网地址");
        AddSuggestion("请检查网络连接，确保已连接到校园网，如果在路由器下使用，请忽略");
        return "未检测到校园网地址";
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