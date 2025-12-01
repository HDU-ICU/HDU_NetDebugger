namespace HDU_NetDebugger.Models;

class GenericConst
{
    // Ping相关
    public const int PingTimeout = 1000; // 超时时间，单位毫秒
    public const int PingBufferSize = 32; // 缓冲区大小，单位字节
    public const int DefaultPingCount = 4; // 默认发送的Ping次数
    public const int PingTtl = 64; // 生存时间（TTL）
}