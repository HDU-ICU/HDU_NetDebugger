using System.Net;
using System.Threading.Tasks;
using HDU_NetDebugger.Attributes;
using HDU_NetDebugger.Models;
using HDU_NetDebugger.Utils;

namespace HDU_NetDebugger.Checkers;

[Checker("检查运营商网关可达性", 30)]
[CheckerCondition("运营商ID非空且有效", nameof(IsISPAvailableAsync))]
public class ISPCheck : CheckerBase
{
    public static async Task<bool> IsISPAvailableAsync()
    {
        if (GlobalFlagList.FlagList.TryGetValue("ISPId", out var ispIdObj) && ispIdObj is short ispId)
        {
            if (ispId <= 0 || !HDUConst.SrunProductIdToGatewayMap.ContainsKey(ispId))
            {
                return false;
            }
            return true;
        }
        return false;
    }

    protected override async Task ExecuteCoreAsync()
    {
        short ispId = (short)GlobalFlagList.FlagList["ISPId"];
        var gatewayIp = HDUConst.SrunProductIdToGatewayMap[ispId];
        var ispName = HDUConst.SrunProductIdToNameMap[ispId];
        DetailsBuilder.AppendLine($"检测到当前使用的运营商为 {ispName}，网关IP为 {gatewayIp}。开始检测网关可达性...");

        var multiPingResult = MultiPing.PingMultiple(gatewayIp, DetailsBuilder);
        if (multiPingResult.SuccessCount == GenericConst.DefaultPingCount)
        {
            Pass($"{ispName}网关可达");
        }
        else if (multiPingResult.SuccessCount > 0)
        {
            AddWarning($"{ispName}网关可达，但是存在丢包，丢包率 {multiPingResult.PacketLossRate * 100}%，平均延迟 {multiPingResult.AvaRoundtripTime} ms");
            AddSuggestion("请检查网线等硬件，或联系ICU");
            Warn($"{ispName}网关可达，但存在丢包");
        }
        else
        {
            AddWarning($"{ispName}网关不可达。");
            AddSuggestion("请联系ICU并提供错误信息");
            Fail($"{ispName}网关不可达");
        }
    }
}