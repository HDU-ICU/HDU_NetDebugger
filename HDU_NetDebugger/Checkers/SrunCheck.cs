using System.Net;
using System.Threading.Tasks;
using HDU_NetDebugger.Attributes;
using HDU_NetDebugger.Models;
using HDU_NetDebugger.Utils;

namespace HDU_NetDebugger.Checkers;

[Checker("检查生活区深澜认证可达性", 15)]
public class SrunCheck : CheckerBase
{
    protected override async Task ExecuteCoreAsync()
    {
        var multiPingResult = MultiPing.PingMultiple(HDUConst.SrunIp, DetailsBuilder);
        if (multiPingResult.SuccessCount == GenericConst.DefaultPingCount)
        {
            Pass("深澜认证服务器可达");
        }
        else if (multiPingResult.SuccessCount > 0)
        {
            AddWarning($"深澜认证服务器可达，但是存在丢包，丢包率 {multiPingResult.PacketLossRate * 100}%，平均延迟 {multiPingResult.AvaRoundtripTime} ms");
            AddSuggestion("请检查网线等硬件，或联系ICU");
            Warn("深澜认证服务器可达，但存在丢包");
        }
        else
        {
            AddWarning("深澜认证服务器不可达。");
            Fail("深澜认证服务器不可达");
        }
    }
}