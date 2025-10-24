using System;

namespace HDU_NetDebugger.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CheckerAttribute : Attribute
{
    public string Name { get; }
    public int Order { get; }
    public CheckerAttribute(string name, int order = 0)
    {
        Name = name;
        Order = order;
    }
}