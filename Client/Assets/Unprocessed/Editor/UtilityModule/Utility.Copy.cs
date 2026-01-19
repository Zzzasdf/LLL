using System;
using System.Reflection;
using UnityEngine;

namespace Editor.UtilityModule
{
    public static partial class Utility
    {
        public static class Copy
        {
            /// 浅拷贝
            public static T Shallow<T>(T source)
            {   
                var target = (T)Activator.CreateInstance(typeof(T));
                Shallow(source, target);
                return target;
            }
            
            /// 浅拷贝
            public static void Shallow<T>(T source, T target)
            {
                if (target == null || source == null)
                {
                    return;
                }
                var type = source.GetType();  
                // 遍历源对象的所有字段  
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))  
                {  
                    field.SetValue(target, field.GetValue(source));  
                }  
                // 遍历源对象的所有属性（假设属性有setter）  
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))  
                {  
                    if (property.CanWrite)  
                    {  
                        property.SetValue(target, property.GetValue(source, null), null);  
                    }  
                }  
            }

            /// 深拷贝 (必须为可写字段)
            public static T Deep<T>(T source)
            {
                var json = JsonUtility.ToJson(source);
                return JsonUtility.FromJson<T>(json);
            }
        }
    }
}
