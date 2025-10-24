using System;
namespace HDU_NetDebugger.Models;

public class CheckResult
{
    public bool IsSuccessful { get; set; }
    public required string Details { get; set; }
}