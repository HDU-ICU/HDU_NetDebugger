using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections.Generic;

namespace HDU_NetDebugger.Models;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(SrunAuthResponse))]
[JsonSerializable(typeof(OnlineDeviceDetail))]
[JsonSerializable(typeof(Dictionary<string, DeviceItem>))]
[JsonSerializable(typeof(DeviceItem))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
    // 不需要手动创建Default实例，源生成器会自动生成
}