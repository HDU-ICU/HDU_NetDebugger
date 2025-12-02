using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using System.Threading.Tasks;
using HDU_NetDebugger.Attributes;
using HDU_NetDebugger.Models;
using HDU_NetDebugger.Utils;

namespace HDU_NetDebugger.Checkers;

[Checker("检查认证状态", 25)]
public class AuthCheck : CheckerBase
{
    protected override async Task ExecuteCoreAsync()
    {
        string host = HDUConst.SrunIp.ToString();
        if (GlobalFlagList.FlagList["DNSStatus"] is bool dnsStatus && dnsStatus)
        {
            DetailsBuilder.AppendLine("DNS 可用，使用域名进行认证状态请求。");
            host = HDUConst.HDUPortalDomain;
        }
        else
        {
            DetailsBuilder.AppendLine("DNS 不可用，使用 IP 地址进行认证状态请求。");
        }
        var srunAuthUrl = "https://" + host + HDUConst.HDUPortalRoute;
        DetailsBuilder.AppendLine($"请求 URL: {srunAuthUrl}");
        var result = await HttpUtils.GetAsync(srunAuthUrl);
        var srunAuthResponse = SrunToJsonDeserializer(result.Content.ReadAsStringAsync().Result);
        if (srunAuthResponse is null)
        {
            AddWarning("无法解析认证响应");
            AddSuggestion("请联系ICU并提供错误信息");
            DetailsBuilder.AppendLine("响应原文为:" + result.Content.ReadAsStringAsync().Result);
            Fail("认证状态查询失败");
            return;
        }

        // 记录运营商信息供后续检查使用
        GlobalFlagList.FlagList["ISPId"] = short.TryParse(srunAuthResponse.ProductsId, out var ispId)
            ? ispId
            : (short)-1;

        var onlineDevicesNotEmpty = GetOnlineDeviceNotEmpty(srunAuthResponse);
        DetailsBuilder.AppendLine("认证响应内容:");
        DetailsBuilder.AppendLine($"运营商: {srunAuthResponse.BillingName}");
        DetailsBuilder.AppendLine($"认证方式: {srunAuthResponse.Domain}");
        DetailsBuilder.AppendLine($"错误信息: {srunAuthResponse.Error}");
        DetailsBuilder.AppendLine($"在线设备数量: {srunAuthResponse.OnlineDeviceTotal}");
        DetailsBuilder.AppendLine($"在线设备(非空)数量: {onlineDevicesNotEmpty.Count}");
        DetailsBuilder.AppendLine("在线设备详情:");
        foreach (var device in onlineDevicesNotEmpty)
        {
            DetailsBuilder.AppendLine($"id: {device.Key}");
            DetailsBuilder.AppendLine($"- 设备IP: {device.Value.Ip}");
            DetailsBuilder.AppendLine($"- 设备IPv6: {device.Value.Ip6}");
            DetailsBuilder.AppendLine($"- 设备系统: {device.Value.OsName}");
            DetailsBuilder.AppendLine(string.Empty);
        }
        if (srunAuthResponse.Error != HDUConst.SrunErrCode.Ok)
        {
            if (srunAuthResponse.Error == HDUConst.SrunErrCode.NotOnline)
            {
                AddWarning("当前未认证");
                AddSuggestion($"请前往认证页面进行认证，网址为 {srunAuthUrl}");
                Fail("未认证");
                return;
            }
            AddWarning($"认证错误: {srunAuthResponse.Error}");
            AddSuggestion($"请尝试重新认证，网址为 {srunAuthUrl}");
            AddSuggestion("如问题依旧，请联系ICU并提供错误信息");
            Fail("认证状态异常");
            return;
        }
        Pass("认证状态正常");
    }

    private static SrunAuthResponse? SrunToJsonDeserializer(string srunResponse)
    {
        // 去掉 jQuery(...) 包装
        int startIndex = srunResponse.IndexOf('(');
        int endIndex = srunResponse.LastIndexOf(')');
        if (startIndex >= 0 && endIndex > startIndex)
        {
            string jsonString = srunResponse.Substring(startIndex + 1, endIndex - startIndex - 1);
            Console.WriteLine("Extracted JSON: " + jsonString);
            return System.Text.Json.JsonSerializer.Deserialize(jsonString, Models.AppJsonSerializerContext.Default.SrunAuthResponse);
        }
        return null;
    }

    private OnlineDeviceDetail GetOnlineDeviceNotEmpty(SrunAuthResponse? srunAuthResponse)
    {
        if (srunAuthResponse is null)
        {
            return new OnlineDeviceDetail();
        }
        var onlineDeviceDetail = srunAuthResponse.OnlineDeviceDetail;
        OnlineDeviceDetail devices = [];
        foreach (var device in onlineDeviceDetail!)
        {
            if (device.Value is not null)
            {
                if (device.Value.Ip != String.Empty)
                {
                    devices[device.Key] = device.Value;
                }
            }
        }
        return devices;
    }
}