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
}