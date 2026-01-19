using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Editor.UtilityModule
{
    public static partial class Utility
    {
        public static class Type
        {
            /// 获取指定基类的派生类
            public static List<System.Type> GetDerivedClasses(System.Type baseType, Assembly assembly)
            {
                return assembly.GetTypes()
                    .Where(type => baseType.IsSubclassOf(type) && !type.IsAbstract)
                    .ToList();
            }
            
            /// 获取实现指定接口的类
            public static IEnumerable<TInterface> GetImplementingClasses<TInterface>(Assembly assembly) where TInterface: class
            {
                var types = assembly.GetTypes()
                    .Where(type => typeof(TInterface).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    .ToList();
            
                var instances = new List<TInterface>();
 
                foreach (var type in types)
                {
                    try
                    {
                        // 尝试使用无参数构造函数创建实例
                        var instance = Activator.CreateInstance(type) as TInterface;
                        if (instance != null)
                        {
                            instances.Add(instance);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 处理无法创建实例的情况（例如，缺少无参数构造函数）
                        Console.WriteLine($"无法创建 {type.FullName} 的实例: {ex.Message}");
                    }
                }
                return instances;
            }

            /// 获取所有带有 指定 特性的脚本 + 筛选条件
            public static bool TryGetAllClassByCustomAttribute<TAttribute>(Assembly assembly, Func<System.Type, bool> filterCondition, out List<System.Type> typesWithAttribute)
                where TAttribute: Attribute
            {
                List<System.Type> types;
                if (!TryGetAllClassByCustomAttribute<TAttribute>(assembly, out types))
                {
                    typesWithAttribute = null;
                    return false;
                }
                typesWithAttribute = new List<System.Type>();
                for (int i = 0; i < types.Count; i++)
                {
                    System.Type type = types[i];
                    if (!filterCondition.Invoke(type))
                    {
                        continue;
                    }
                    typesWithAttribute.Add(type);
                }
                if (typesWithAttribute.Count == 0)
                {
                    typesWithAttribute = null;
                    return false;
                }
                return true;
            }
            
            /// 获取所有带有 指定 特性的脚本
            public static bool TryGetAllClassByCustomAttribute<TAttribute>(Assembly assembly, out List<System.Type> typesWithAttribute) 
                where TAttribute : Attribute
            {
                List<System.Type> tmp = null;
                // 遍历程序集中的所有类型
                foreach (System.Type type in assembly.GetTypes())
                {
                    // 检查类型是否应用了指定的自定义特性
                    if (type.GetCustomAttribute<TAttribute>() == null)
                    {
                        continue;
                    }
                    tmp ??= new List<System.Type>();
                    tmp.Add(type);
                }
                if (tmp == null)
                {
                    typesWithAttribute = null;
                    return false;
                }
                typesWithAttribute = tmp;
                return true;
            }
        }
    }
}
