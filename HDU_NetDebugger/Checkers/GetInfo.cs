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
public class GetInfo : IChecker
{
    public required CheckResult Result;
    async Task<CheckResult> IChecker.ExecuteAsync()
    {
        return await Task.Run(() =>
        {
            Result = new CheckResult
            {
                Status = CheckResultStatus.Pass,
                Summary = "基本信息获取成功",
                Details = string.Empty
            };
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            if (networkInterfaces.Length == 0)
            {
                Result.Status = CheckResultStatus.Fail;
                Result.Summary = "未获取到网络接口信息";
                Result.Details = string.Empty;
                return Result;
            }
            foreach (var ni in networkInterfaces)
            {
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    HasIPv4Address(ni))
                {
                    Result.Details += BuildIFInfo(ni) + "\n";
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
            if (string.IsNullOrEmpty(Result.Details))
            {
                Result.Status = CheckResultStatus.Fail;
                Result.Summary = "未找到符合条件的网络接口";
                Result.Details = "未找到符合条件的网络接口（以太网且已启用且有IPv4地址）\n";
            }
            Result.Details = Result.Details.TrimEnd('\n');
            Result.Summary = GetSummary();
            return Result;
        });
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
            if (netType == "Dorm")
            {
                return "检测到生活区网络";
            }
            else if (netType == "Campus")
            {
                return "检测到教学区网络";
            }
        }
        RaiseWarning(String.Empty);
        return "未检测到校园网地址，如有路由器请忽略";
    }
    private bool CheckNetworkType(NetworkInterface ni)
    {
        if (HasDormIPv4Address(ni))
        {
            if (CheckNetworkTypeExists(ni))
            {
                RaiseWarning("检测到多个校园网地址，可能存在网络配置问题");
            }
            GlobalFlagList.FlagList["NetWorkType"] = "Dorm";
            return true;
        }
        else if (HasCampusIPv4Address(ni))
        {
            if (CheckNetworkTypeExists(ni))
            {
                RaiseWarning("检测到多个校园网地址，可能存在网络配置问题");
            }
            GlobalFlagList.FlagList["NetWorkType"] = "Campus";
            return true;
        }
        return false;
    }
    private static bool CheckNetworkTypeExists(NetworkInterface ni)
    {
        return GlobalFlagList.FlagList.ContainsKey("NetWorkType");
    }
    private void RaiseWarning(string message)
    {
        if (Result.Status != CheckResultStatus.Fail)
        {
            Result.Status = CheckResultStatus.Warn;
        }
        Result.Summary += $"警告: {message}\n";
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