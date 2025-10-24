using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HDU_NetDebugger.Models;

namespace HDU_NetDebugger.Services;

public class CheckServices
{
    public static IEnumerable<(IChecker? Checker, Dictionary<string, Func<Task<bool>>> Conditions)> GetAvailableCheckers(params string[] targetNamespaces)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        if (targetNamespaces.Length > 0)
        {
            types = types.Where(t => targetNamespaces.Any(ns => t.Namespace?.StartsWith(ns) == true)).ToArray();
        }

        return types
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IChecker).IsAssignableFrom(t))
            .OrderBy(t => t.GetCustomAttribute<CheckerAttribute>()?.Order ?? 0)
            .Select(t =>
            {
                var checker = Activator.CreateInstance(t) as IChecker;
                var conditions = new Dictionary<string, Func<Task<bool>>>();
                var conditionAttributes = t.GetCustomAttributes<CheckerCondition>();
                foreach (var conditionAttr in conditionAttributes)
                {
                    conditions[conditionAttr.Name] = conditionAttr.ConditionFactory;
                }

                return (checker, conditions);
            })
            .ToList();
    }

    /// <summary>
    /// 获取指定命名空间下的检查器
    /// </summary>
    /// <param name="namespace">目标命名空间</param>
    /// <returns>检查器实例和条件的元组集合</returns>
    public static IEnumerable<(IChecker? Checker, Dictionary<string, Func<Task<bool>>> Conditions)> GetCheckersByNamespace(string @namespace)
    {
        return GetAvailableCheckers(@namespace);
    }

    /// <summary>
    /// 获取默认命名空间下的检查器（HDU_NetDebugger 命名空间）
    /// </summary>
    /// <returns>检查器实例和条件的元组集合</returns>
    public static IEnumerable<(IChecker? Checker, Dictionary<string, Func<Task<bool>>> Conditions)> GetDefaultCheckers()
    {
        return GetAvailableCheckers("HDU_NetDebugger");
    }
}
