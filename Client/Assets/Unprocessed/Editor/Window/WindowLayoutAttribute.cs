using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorModule
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WindowLayoutAttribute : Attribute
    {
        public static WindowLayoutGenerator Generator { get; } = new WindowLayoutGenerator();
        private Type Parent { get; }
        private string TabName { get; }

        public WindowLayoutAttribute(Type parent = null, string tabName = null)
        {
            this.Parent = parent;
            this.TabName = tabName;
        }

        private Type currentType;
        private WindowCollectorUtilityConfig config;
        
        private void OnGUI(Type currentType, WindowCollectorUtilityConfig config)
        {
            this.currentType = currentType;
            this.config = config;
            Init();
            if (subTypes == null) return;
            using (new EditorGUILayout.VerticalScope())
            {
                OnRenderSubTab();
                OnRenderSubSelected();
            }
        }
        
        private bool init;
        private List<Type> subTypes;
        private void Init()
        {
            if (init) return;
            init = !init;
            if (currentType == null)
            {
                subTypes = null;
                return;
            }
            Editor.UtilityModule.Utility.Type.TryGetAllClassByCustomAttribute<WindowLayoutAttribute>(Assembly.GetExecutingAssembly(), 
                 type =>
                {
                    WindowLayoutAttribute attribute = Generator.GetUniqueAttr(type);
                    return attribute.Parent == currentType;
                }, 
                out subTypes);
        }
        
        private int selectedIndex;
        private void OnRenderSubTab()
        {
            if (subTypes == null) return;
            if (config.HideList) return;
            string[] tabs = subTypes.Select(x =>
            {
                WindowLayoutAttribute parentRootAttribute = Generator.GetUniqueAttr(x);
                if (string.IsNullOrEmpty(parentRootAttribute.TabName))
                {
                    return x.Name;
                }
                return parentRootAttribute.TabName;
            }).ToArray();
            selectedIndex = GUILayout.SelectionGrid(Mathf.Min(selectedIndex, tabs.Length), tabs, tabs.Length, GUILayout.ExpandWidth(false));
        }
        
        private void OnRenderSubSelected()
        {
            if (subTypes == null) return;
            Type subType = subTypes[selectedIndex];
            // 实体
            {
                var instance = Generator.GetUniqueObj(subType);
                if(instance != null)
                {
                    MethodInfo methodInfo = subType.GetMethod("OnGUI", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    methodInfo?.Invoke(instance, null);
                } 
            }
            // 特性
            {
                var attribute = Generator.GetUniqueAttr(subType);
                if(attribute != null)
                {
                    MethodInfo methodInfo = typeof(WindowLayoutAttribute).GetMethod("OnGUI", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    methodInfo?.Invoke(attribute, new object[]{ subType, config });
                }
            }
        }
        
        public class WindowLayoutGenerator
        {
            private Dictionary<Type, WindowLayoutAttribute> attrInstances;
            public WindowLayoutAttribute GetUniqueAttr(Type type)
            {
                attrInstances ??= new Dictionary<Type, WindowLayoutAttribute>();
                if (!attrInstances.TryGetValue(type, out WindowLayoutAttribute attribute))
                {
                    attrInstances.Add(type, attribute = type.GetCustomAttribute<WindowLayoutAttribute>());
                }
                return attribute;
            }
            
            private Dictionary<Type, object> objInstances;
            public object GetUniqueObj(Type type)
            {
                objInstances ??= new Dictionary<Type, object>();
                if (!objInstances.TryGetValue(type, out object obj))
                {
                    objInstances.Add(type, obj = Activator.CreateInstance(type));
                }
                return obj;
            }
        }
    }
}
