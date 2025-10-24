using System;

namespace HDU_NetDebugger.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CheckerCondition : Attribute
{
    public string Name { get; }
    public string ConditionMethodName { get; set; }

    public CheckerCondition(string name, string conditionMethodName)
    {
        Name = name;
        ConditionMethodName = conditionMethodName;
    }
}