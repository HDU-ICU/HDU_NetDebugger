using System;
using System.Threading.Tasks;

namespace HDU_NetDebugger.Services;

[AttributeUsage(AttributeTargets.Class)]
public class CheckerConditionAttribute : Attribute
{
    public string Name { get; }
    public Task<bool> Condition { get; set; }
    public CheckerConditionAttribute(string name, Task<bool> condition)
    {
        Name = name;
        Condition = condition;
    }
}