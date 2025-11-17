using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace HDU_NetDebugger.Models;

public class CheckResult
{
    public CheckResultStatus Status { get; set; }
    public required string Summary { get; set; }
    public string? Details { get; set; }
    public List<string> Warnings { get; set; } = [];
    public List<string> Suggestions { get; set; } = [];
}