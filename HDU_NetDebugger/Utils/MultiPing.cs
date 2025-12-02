using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using HDU_NetDebugger.Models;

namespace HDU_NetDebugger.Utils;

class MultiPing()
{
    private static readonly Ping _ping = new();
    private static readonly PingOptions _pingOptions = new(GenericConst.PingTtl, true);

    public static MultiPingResult PingMultiple(IPAddress targetAddress, StringBuilder? detailBuilder = null, int count = GenericConst.DefaultPingCount)
    {
        int successCount = 0;
        long totalRoundtripTime = 0;
        long minRoundtripTime = long.MaxValue;
        long maxRoundtripTime = long.MinValue;

        detailBuilder?.AppendLine($"开始对 {targetAddress} 进行 {count} 次 Ping 测试...");
        for (int i = 0; i < count; i++)
        {
            PingReply reply = _ping.Send(targetAddress, GenericConst.PingTimeout, new byte[GenericConst.PingBufferSize], _pingOptions);
            if (reply.Status == IPStatus.Success)
            {
                successCount++;
                totalRoundtripTime += reply.RoundtripTime;
                if (reply.RoundtripTime < minRoundtripTime)
                {
                    minRoundtripTime = reply.RoundtripTime;
                }
                if (reply.RoundtripTime > maxRoundtripTime)
                {
                    maxRoundtripTime = reply.RoundtripTime;
                }
            }
            detailBuilder?.AppendLine($"Ping {targetAddress} 第 {i + 1} 次: 状态:{reply.Status}, 时间:{reply.RoundtripTime}ms");
        }
        // 防止最大和最小值未更新的情况
        if (minRoundtripTime == long.MaxValue) minRoundtripTime = 0;
        if (maxRoundtripTime == long.MinValue) maxRoundtripTime = 0;
        long averageRoundtripTime = successCount > 0 ? totalRoundtripTime / successCount : 0;
        detailBuilder?.AppendLine($"Ping 测试完成: 成功次数:{successCount}/{count}, 平均时间:{averageRoundtripTime}ms, 最小时间:{minRoundtripTime}ms, 最大时间:{maxRoundtripTime}ms");

        return new MultiPingResult
        {
            SuccessCount = successCount,
            TotalCount = count,
            AvaRoundtripTime = averageRoundtripTime,
            TotalRoundtripTime = totalRoundtripTime,
            MinRoundtripTime = minRoundtripTime,
            MaxRoundtripTime = maxRoundtripTime
        };
    }
}


public struct MultiPingResult
{
    public int SuccessCount;
    public int TotalCount;
    public long TotalRoundtripTime;
    public long AvaRoundtripTime;
    public long MinRoundtripTime;
    public long MaxRoundtripTime;
    public readonly double PacketLossRate => TotalCount > 0 ? (double)(TotalCount - SuccessCount) / TotalCount : 0;
}