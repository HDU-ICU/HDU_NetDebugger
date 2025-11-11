using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HDU_NetDebugger.Attributes;
using HDU_NetDebugger.Checkers;

namespace HDU_NetDebugger.Services;

public class CheckServices
{
    public static IEnumerable<(string Name, IChecker? Checker, Dictionary<string, Func<Task<bool>>> Conditions)> GetAvailableCheckers(params string[] targetNamespaces)
    {
        // 获取所有已加载的程序集
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(a =>
        {
            try
            {
                return a.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // 处理加载异常，返回成功加载的类型
                return ex.Types.Where(t => t != null).Cast<Type>();
            }
        }).ToArray();

        // 如果指定了目标命名空间，则进行筛选
        if (targetNamespaces.Length > 0)
        {
            types = types.Where(t => t.Namespace != null && targetNamespaces.Any(ns => t.Namespace.StartsWith(ns))).ToArray();
        }

        // 获取所有符合条件的checker类型，并按名称分组处理重名情况
        var checkerTypes = types
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IChecker).IsAssignableFrom(t) && t.GetCustomAttribute<CheckerAttribute>() != null)
            .ToList();

        // 按checker名称分组，并在每个组内选择命名空间最深的类型
        var selectedTypes = checkerTypes
            .GroupBy(t => t.GetCustomAttribute<CheckerAttribute>()?.Name ?? t.Name)
            .Select(group =>
            {
                // 在相同名称的组内，选择命名空间最深的类型
                // 使用更优雅的LINQ方式计算命名空间深度（点的数量）
                return group.OrderByDescending(t => t.Namespace?.Count(c => c == '.') ?? -1).First();
            })
            .OrderBy(t => t.GetCustomAttribute<CheckerAttribute>()?.Order ?? 0);

        return selectedTypes
            .Select(t =>
            {
                var checkerAttribute = t.GetCustomAttribute<CheckerAttribute>();
                IChecker? checker = null;
                try
                {
                    checker = Activator.CreateInstance(t) as IChecker;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to create instance of {t.FullName}: {ex.Message}");
                }

                var conditions = new Dictionary<string, Func<Task<bool>>>();
                var conditionAttributes = t.GetCustomAttributes<CheckerCondition>();
                foreach (var conditionAttr in conditionAttributes)
                {
                    try
                    {
                        var methodInfo = t.GetMethod(conditionAttr.ConditionMethodName, BindingFlags.Public | BindingFlags.Static);
                        if (methodInfo != null && methodInfo.ReturnType == typeof(Task<bool>))
                        {
                            var conditionFactory = (Func<Task<bool>>)Delegate.CreateDelegate(typeof(Func<Task<bool>>), methodInfo);
                            conditions[conditionAttr.Name] = conditionFactory;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to create delegate for condition {conditionAttr.Name} in {t.FullName}: {ex.Message}");
                    }
                }

                return (checkerAttribute?.Name ?? t.Name, checker, conditions);
            })
            .ToList();
    }

}
