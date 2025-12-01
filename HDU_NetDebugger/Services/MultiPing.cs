using System.Net;
using System.Net.NetworkInformation;
using HDU_NetDebugger.Models;

namespace HDU_NetDebugger.Services;

class MultiPing(IPAddress targetAddress)
{
    private readonly Ping _ping = new();
    private readonly PingOptions _pingOptions = new(GenericConst.PingTtl, true);
    private readonly IPAddress _address = targetAddress;
    public MultiPingResult PingMultiple(int count = GenericConst.DefaultPingCount)
    {
        int successCount = 0;
        long totalRoundtripTime = 0;
        long minRoundtripTime = long.MaxValue;
        long maxRoundtripTime = long.MinValue;

        for (int i = 0; i < count; i++)
        {
            PingReply reply = _ping.Send(_address, GenericConst.PingTimeout, new byte[GenericConst.PingBufferSize], _pingOptions);
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
        }
        long averageRoundtripTime = successCount > 0 ? totalRoundtripTime / successCount : 0;

        return new MultiPingResult
        {
            SuccessCount = successCount,
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
    public long TotalRoundtripTime;
    public long AvaRoundtripTime;
    public long MinRoundtripTime;
    public long MaxRoundtripTime;
}