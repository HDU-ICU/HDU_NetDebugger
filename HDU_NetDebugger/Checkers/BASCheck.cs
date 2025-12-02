using System.Net;
using System.Threading.Tasks;
using HDU_NetDebugger.Attributes;
using HDU_NetDebugger.Models;
using HDU_NetDebugger.Utils;

namespace HDU_NetDebugger.Checkers;

[Checker("检查BAS可达性", 10)]
public class BASCheck : CheckerBase
{
    protected override async Task ExecuteCoreAsync()
    {
        var multiPingResult = MultiPing.PingMultiple(HDUConst.BASIp, DetailsBuilder);
        if (multiPingResult.SuccessCount == GenericConst.DefaultPingCount)
        {
            Pass("BAS 可达");
        }
        else if (multiPingResult.SuccessCount > 0)
        {
            AddWarning($"BAS 可达，但是存在丢包，丢包率 {multiPingResult.PacketLossRate * 100}%，平均延迟 {multiPingResult.AvaRoundtripTime} ms");
            AddSuggestion("请检查网线等硬件，或联系ICU");
            Warn("BAS 可达，但存在丢包");
        }
        else
        {
            AddWarning("BAS 不可达。");
            var httpResponse = await HttpUtils.GetAsync(GenericConst.MiuiRom204Url);
            if (httpResponse is { StatusCode: HttpStatusCode.NoContent })
            {
                AddWarning("您可能不是在使用校园网");
                AddSuggestion("请切换到校园网环境后重试");
            }
            else
            {
                AddSuggestion("请检查IP配置或尝试重新进行物理连接");
            }
            Fail("BAS 不可达");
        }
    }
}