namespace HDU_NetDebugger.Models;

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// 根对象
/// </summary>
public class SrunAuthResponse
{
    // 缓存JsonSerializerOptions实例，避免每次创建新实例
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    [JsonPropertyName("ServerFlag")]        // Newtonsoft 用 [JsonProperty]
    public long ServerFlag { get; set; }

    [JsonPropertyName("add_time")]
    public long AddTime { get; set; }

    [JsonPropertyName("all_bytes")]
    public long AllBytes { get; set; }

    [JsonPropertyName("billing_name")]
    public string BillingName { get; set; } = string.Empty;

    [JsonPropertyName("bytes_in")]
    public long BytesIn { get; set; }

    [JsonPropertyName("bytes_out")]
    public long BytesOut { get; set; }

    [JsonPropertyName("checkout_date")]
    public long CheckoutDate { get; set; }

    [JsonPropertyName("domain")]
    public string Domain { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;

    [JsonPropertyName("group_id")]
    public string GroupId { get; set; } = string.Empty;

    [JsonPropertyName("keepalive_time")]
    public long KeepaliveTime { get; set; }

    /// <summary>
    /// 直接反序列化会因为转义失败报错，所以先用 string 接收原始数据
    /// </summary>
    [JsonPropertyName("online_device_detail")]
    public string OnlineDeviceDetailRaw { get; set; } = string.Empty;

    // 只读属性，二次反序列化
    [JsonIgnore]
    public OnlineDeviceDetail? OnlineDeviceDetail =>
        string.IsNullOrEmpty(OnlineDeviceDetailRaw)
            ? null
            : JsonSerializer.Deserialize<OnlineDeviceDetail>(OnlineDeviceDetailRaw, _jsonOptions);

    [JsonPropertyName("online_device_total")]
    public string OnlineDeviceTotal { get; set; } = string.Empty;

    [JsonPropertyName("online_ip")]
    public string OnlineIp { get; set; } = string.Empty;

    [JsonPropertyName("online_ip6")]
    public string OnlineIp6 { get; set; } = string.Empty;

    [JsonPropertyName("package_id")]
    public string PackageId { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("pppoe_dial")]
    public string PppoeDial { get; set; } = string.Empty;

    [JsonPropertyName("products_id")]
    public short ProductsId { get; set; }

    [JsonPropertyName("products_name")]
    public string ProductsName { get; set; } = string.Empty;

    [JsonPropertyName("real_name")]
    public string RealName { get; set; } = string.Empty;

    [JsonPropertyName("remain_bytes")]
    public long RemainBytes { get; set; }

    [JsonPropertyName("remain_seconds")]
    public long RemainSeconds { get; set; }

    [JsonPropertyName("sum_bytes")]
    public long SumBytes { get; set; }

    [JsonPropertyName("sum_seconds")]
    public long SumSeconds { get; set; }

    [JsonPropertyName("sysver")]
    public string Sysver { get; set; } = string.Empty;

    [JsonPropertyName("user_balance")]
    public long UserBalance { get; set; }

    [JsonPropertyName("user_charge")]
    public long UserCharge { get; set; }

    [JsonPropertyName("user_mac")]
    public string UserMac { get; set; } = string.Empty;

    [JsonPropertyName("user_name")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("wallet_balance")]
    public long WalletBalance { get; set; }
}

/// <summary>
/// 如果你想把 online_device_detail 继续拆开，就再用这个类
/// key 就是字符串 rad_online_id，value 是设备信息
/// </summary>
public class OnlineDeviceDetail : Dictionary<string, DeviceItem> { }

public class DeviceItem
{
    [JsonPropertyName("class_name")]
    public string ClassName { get; set; } = string.Empty;

    [JsonPropertyName("ip")]
    public string Ip { get; set; } = string.Empty;

    [JsonPropertyName("ip6")]
    public string Ip6 { get; set; } = string.Empty;

    [JsonPropertyName("os_name")]
    public string OsName { get; set; } = string.Empty;

    [JsonPropertyName("rad_online_id")]
    public string RadOnlineId { get; set; } = string.Empty;
}
