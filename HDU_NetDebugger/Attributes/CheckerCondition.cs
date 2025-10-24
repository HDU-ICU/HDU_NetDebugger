using System;
using System.Threading.Tasks;

namespace HDU_NetDebugger.Services;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CheckerCondition : Attribute
{
    public string Name { get; }
    public Func<Task<bool>> ConditionFactory { get; set; }
    public CheckerCondition(string name, Func<Task<bool>> conditionFactory)
    {
        Name = name;
        ConditionFactory = conditionFactory;
    }
}