using System.Net;
using System.Threading.Tasks;
using HDU_NetDebugger.Attributes;
using HDU_NetDebugger.Models;
using HDU_NetDebugger.Utils;

namespace HDU_NetDebugger.Checkers;

[Checker("检查公网可达性", 35)]
public class PublicNetCheck : CheckerBase
{
    protected override async Task ExecuteCoreAsync()
    {
        var multiPingResult = MultiPing.PingMultiple(GenericConst.AliDnsIp, DetailsBuilder);
        if (multiPingResult.SuccessCount == GenericConst.DefaultPingCount)
        {
            DetailsBuilder.AppendLine("连接到阿里主DNS成功，公网可达。");
        }
        else if (multiPingResult.SuccessCount > 0)
        {
            AddWarning($"阿里主DNS可达，但是存在丢包，丢包率 {multiPingResult.PacketLossRate * 100}%，平均延迟 {multiPingResult.AvaRoundtripTime} ms");
            AddSuggestion("请检查网线等硬件，或联系ICU");
            Warn("阿里主DNS可达，但存在丢包");
        }
        else
        {
            AddWarning("阿里主DNS不可达。");
        }
        var httpResponse = await HttpUtils.GetAsync(GenericConst.MiuiRom204Url);
        if (httpResponse is { StatusCode: HttpStatusCode.NoContent })
        {
            Pass("公网可达");
            return;
        }
        else
        {
            AddWarning("无法通过HTTP访问公网地址。");
            AddSuggestion("请检查IP配置或尝试重新进行物理连接");
        }
        Fail("公网不可达");
    }
}