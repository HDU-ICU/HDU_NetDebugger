using System.Net.Http;
using System.Threading.Tasks;

namespace HDU_NetDebugger.Utils;

class HttpUtils
{
    private static readonly HttpClientHandler _httpClientHandler = new()
    {
        AllowAutoRedirect = false,
        UseCookies = false,
    };
    private static readonly HttpClient _httpClient = new(_httpClientHandler);
    public static async Task<HttpResponseMessage> GetAsync(string url)
    {
        HttpRequestMessage request = new(HttpMethod.Get, url);
        return await _httpClient.SendAsync(request);
    }
}