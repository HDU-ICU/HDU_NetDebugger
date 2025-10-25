using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using HDU_NetDebugger.Attributes;
using HDU_NetDebugger.Models;

namespace HDU_NetDebugger.Checkers;

[Checker("获取基本信息", -1)]
public class GetInfo : IChecker
{
    Task<CheckResult> IChecker.ExecuteAsync()
    {
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        if (networkInterfaces.Length == 0)
        {
            return Task.FromResult(new CheckResult
            {
                Status = CheckResultStatus.Fail,
                Summary = "未获取到网络接口信息",
                Details = String.Empty
            });
        }
        var details = string.Empty;
        foreach (var ni in networkInterfaces)
        {
            if (ni.OperationalStatus == OperationalStatus.Up &&
                ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                details += BuildIFInfo(ni) + "\n";
            }
        }
        return Task.FromResult(new CheckResult
        {
            Status = CheckResultStatus.Pass,
            Summary = "已获取网络接口信息",
            Details = details.TrimEnd('\n')
        });
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