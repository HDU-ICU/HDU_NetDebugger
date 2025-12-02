using System.Net;
using System.Threading.Tasks;
using HDU_NetDebugger.Attributes;
using HDU_NetDebugger.Models;
using HDU_NetDebugger.Utils;
using DnsClient;
using System.Linq;

namespace HDU_NetDebugger.Checkers;

[Checker("检查DNS可用性", 20)]
public class DNSCheck : CheckerBase
{
    protected override async Task ExecuteCoreAsync()
    {
        var client = new LookupClient(HDUConst.HDUDnsIp);
        var result = await client.QueryAsync(HDUConst.HDUPortalDomain, QueryType.A);
        if (result.HasError)
        {
            AddWarning($"DNS 查询失败");
            DetailsBuilder.AppendLine($"DNS查询失败，错误信息: {result.ErrorMessage}");
            Fail("DNS 查询失败");
            return;
        }
        else if (result.Answers.Count == 0)
        {
            AddWarning("DNS 查询未返回任何结果");
            AddSuggestion($"请检查DNS服务器设置，建议把DNS服务器设置为 {HDUConst.HDUDnsIp}");
            Fail("DNS 查询未返回结果");
            return;
        }
        else
        {
            DetailsBuilder.AppendLine("DNS 查询成功，返回结果如下：");
            foreach (var record in result.Answers)
            {
                DetailsBuilder.AppendLine(record.ToString());
            }
            bool hasRightAnswer = result.Answers
               .ARecords()
               .Any(r => r.Address.Equals(HDUConst.SrunIp));
            if (hasRightAnswer)
            {
                Pass("DNS 查询成功，返回了正确的 A 记录");
            }
            else
            {
                AddWarning("DNS 查询未返回正确的记录");
                AddSuggestion($"请检查DNS服务器设置，建议把DNS服务器设置为 {HDUConst.HDUDnsIp}");
                Fail("DNS 查询未返回正确的记录");
            }
        }
    }
}