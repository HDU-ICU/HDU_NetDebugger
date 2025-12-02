using System;
using System.Collections.Generic;
using System.Net;

namespace HDU_NetDebugger.Models;

public static class HDUConst
{
    public static readonly IPNetwork DormNetwork = IPNetwork.Parse("10.150.0.0/16");    // 生活区网段
    public static readonly IPNetwork CampusNetwork = IPNetwork.Parse("10.249.0.0/16");  // 校园网段
    public static readonly IPAddress BASIp = IPAddress.Parse("10.150.0.1");             // BAS
    public static readonly IPAddress SrunIp = IPAddress.Parse("192.168.112.97");        // 深澜认证服务器
    public const string HDUPortalDomain = "portal.hdu.edu.cn";                          // 杭电生活区认证门户域名
    public const string HDUPortalRoute = "/cgi-bin/rad_user_info?callback=jQuery";      // 认证状态查询路由
    public static readonly IPAddress HDUDnsIp = IPAddress.Parse("210.32.32.1");         // DNS

    // 深澜返回的错误码，注意：不全
    public static class SrunErrCode
    {
        public const string Ok = "ok";
        public const string NotOnline = "not_online_error";
    }

    // 深澜的运营商ID对应
    public enum SrunProductId : short
    {
        ChinaUnicom = 3,      // 中国联通
        ChinaTelecom = 4,     // 中国电信        
        ChinaMobile = 5,      // 中国移动
    }

    // ID到名称映射
    public static readonly Dictionary<short, string> SrunProductIdToNameMap = new()
    {
        {(short)SrunProductId.ChinaUnicom, "中国联通" },
        {(short)SrunProductId.ChinaTelecom, "中国电信" },
        {(short)SrunProductId.ChinaMobile, "中国移动" },
    };

    // ID到网关IP映射
    public static readonly Dictionary<short, IPAddress> SrunProductIdToGatewayMap = new()
    {
        {(short)SrunProductId.ChinaUnicom, IPAddress.Parse("172.20.64.1") },
        {(short)SrunProductId.ChinaTelecom, IPAddress.Parse("60.176.40.1") },
        {(short)SrunProductId.ChinaMobile, IPAddress.Parse("10.106.0.1") },
    };
}