using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HDU_NetDebugger.Attributes;

namespace HDU_NetDebugger.Checkers;

[Checker("检查代理状态", 5)]
public class ProxyCheck : CheckerBase
{
    protected override async Task ExecuteCoreAsync()
    {
        proxyCheckers = new Dictionary<string, Func<bool>>()
        {
            { "HttpClient 默认代理", CheckHttpClientDefaultProxy },
            { "环境变量代理", CheckEnvironmentVariableProxy }
        };
        ushort proxyCount = 0;
        foreach (var checker in proxyCheckers)
        {
            DetailsBuilder.AppendLine($"--使用方法 \"{checker.Key}\" 进行代理检测--");
            bool isProxyDetected = checker.Value();
            if (isProxyDetected)
            {
                proxyCount++;
                DetailsBuilder.AppendLine($"检测到代理配置。");
                AddSuggestion($"请检查系统代理设置并关闭代理，或取消相关环境变量的设置。");
            }
            else
            {
                DetailsBuilder.AppendLine("未检测到代理配置。");
            }
            DetailsBuilder.AppendLine();
        }
        if (proxyCount > 0)
        {
            AddWarning("检测到代理配置，可能会影响连接。");
            Fail("检测到代理配置");
        }
        else
        {
            Pass("未检测到代理配置");
        }
    }

    private Dictionary<string, Func<bool>>? proxyCheckers;

    // 检查 HttpClientHandler 是否解析到默认代理。此方法在所有平台上表现一致。
    private bool CheckHttpClientDefaultProxy()
    {
        try
        {
            var handler = new HttpClientHandler();
            if (handler.UseProxy && handler.Proxy != null)
            {
                // HttpClientHandler 在内部根据 OS 规则获取代理。
                // 尝试获取一个示例 URI 的代理，如果返回非空且非本地环回地址，则认为存在有效代理。
                Uri? proxyAddress = null;
                try
                {
                    proxyAddress = handler.Proxy.GetProxy(new Uri("http://example.com"));
                    DetailsBuilder.AppendLine($"获取到的代理地址: {proxyAddress}");
                }
                catch (NotSupportedException)
                {
                    DetailsBuilder.AppendLine("代理对象不支持 GetProxy 方法，无法获取代理地址。");
                    // 有些 IWebProxy 实现可能不支持 GetProxy，直接返回 true
                    // 但通常情况下，如果 UseProxy 为 true，这里应该能工作
                    return true;
                }

                return proxyAddress != null && !proxyAddress.IsLoopback;
            }
            return false;
        }
        catch (Exception ex)
        {
            DetailsBuilder.AppendLine($" 错误: 获取 HttpClient 默认代理时发生错误: {ex.Message}");
            return false;
        }
    }

    // 检查 HTTP_PROXY, HTTPS_PROXY 等环境变量是否设置。此方法在所有平台上表现一致。
    private bool CheckEnvironmentVariableProxy()
    {
        string? httpProxy = Environment.GetEnvironmentVariable("HTTP_PROXY");
        string? httpsProxy = Environment.GetEnvironmentVariable("HTTPS_PROXY");
        // 也检查小写形式，因为 Unix 系统通常使用小写
        string? httpProxyLower = Environment.GetEnvironmentVariable("http_proxy");
        string? httpsProxyLower = Environment.GetEnvironmentVariable("https_proxy");
        DetailsBuilder.AppendLine($"HTTP_PROXY: {httpProxy}");
        DetailsBuilder.AppendLine($"HTTPS_PROXY: {httpsProxy}");
        DetailsBuilder.AppendLine($"http_proxy: {httpProxyLower}");
        DetailsBuilder.AppendLine($"https_proxy: {httpsProxyLower}");
        return !string.IsNullOrEmpty(httpProxy) || !string.IsNullOrEmpty(httpsProxy) ||
               !string.IsNullOrEmpty(httpProxyLower) || !string.IsNullOrEmpty(httpsProxyLower);
    }
}