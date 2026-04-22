using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HDU_NetDebugger.Attributes;
using HDU_NetDebugger.Models;

namespace HDU_NetDebugger.Services;

public static class CheckerCollector
{
    public record CheckerInfo(
        string Name,
        Type CheckerType,
        int Order,
        Dictionary<string, Func<Task<bool>>> Conditions
    );

    private static readonly Dictionary<string, CheckerInfo> _registry = [];

    /// <summary>
    /// 扫描当前 AppDomain 中所有已加载的程序集，注册带有 <see cref="CheckerAttribute"/> 的 <see cref="IChecker"/> 实现。
    /// 同名 Checker 按命名空间深度升序遍历，后面的（更深的）直接覆盖前面的，从而自然保证平台特化版本优先。
    /// </summary>
    public static void DiscoverAndRegister()
    {
        _registry.Clear();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(a =>
        {
            try
            {
                return a.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null).Cast<Type>();
            }
        });

        var checkerTypes = types
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IChecker).IsAssignableFrom(t))
            .Select(t => new { Type = t, Attribute = t.GetCustomAttribute<CheckerAttribute>() })
            .Where(x => x.Attribute != null)
            .ToList();

        // 按命名空间深度升序（先浅后深），同深度再按 Order 排序
        // 同名时后遍历到的（命名空间更深）会覆盖前者，自然实现平台特化优先
        var ordered = checkerTypes
            .OrderBy(x => x.Type.Namespace?.Count(c => c == '.') ?? -1)
            .ThenBy(x => x.Attribute!.Order);

        foreach (var item in ordered)
        {
            var attr = item.Attribute!;
            var conditions = ExtractConditions(item.Type);
            _registry[attr.Name] = new CheckerInfo(
                attr.Name,
                item.Type,
                attr.Order,
                conditions
            );
        }
    }

    private static Dictionary<string, Func<Task<bool>>> ExtractConditions(Type type)
    {
        var conditions = new Dictionary<string, Func<Task<bool>>>();
        var conditionAttributes = type.GetCustomAttributes<CheckerCondition>();
        foreach (var conditionAttr in conditionAttributes)
        {
            try
            {
                var methodInfo = type.GetMethod(conditionAttr.ConditionMethodName, BindingFlags.Public | BindingFlags.Static);
                if (methodInfo != null && methodInfo.ReturnType == typeof(Task<bool>))
                {
                    var conditionFactory = (Func<Task<bool>>)Delegate.CreateDelegate(typeof(Func<Task<bool>>), methodInfo);
                    conditions[conditionAttr.Name] = conditionFactory;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create delegate for condition {conditionAttr.Name} in {type.FullName}: {ex.Message}");
            }
        }
        return conditions;
    }

    /// <summary>
    /// 获取所有已注册的 Checker 信息，按 Order 排序。
    /// </summary>
    public static IEnumerable<CheckerInfo> GetRegistered()
    {
        return _registry.Values.OrderBy(x => x.Order);
    }
}
