using System;
namespace HDU_NetDebugger.Models;

public class CheckResult
{
    public CheckResultStatus Status { get; set; }
    public required string Details { get; set; }
}