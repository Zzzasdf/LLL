using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorModule
{
    [WindowLayout]
    public class WindowCollectorEntry : EditorWindow
    {
        [MenuItem("Tools/WindowCollector")]
        private static void PanelOpen()
        {
            GetWindow<WindowCollectorEntry>();
        }

        private IWindowCollectorUtility windowCollectorUtility;

        private Vector2 scroll;
        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            windowCollectorUtility ??= new WindowCollectorUtility();
            windowCollectorUtility.OnGUI(() =>
            {
                WindowLayoutAttribute attribute = WindowLayoutAttribute.Generator.GetUniqueAttr(typeof(WindowCollectorEntry));
                MethodInfo methodInfo = typeof(WindowLayoutAttribute).GetMethod("OnGUI", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                methodInfo?.Invoke(attribute, new object[]{ typeof(WindowCollectorEntry), windowCollectorUtility.GetConfig() });
            });
            EditorGUILayout.EndScrollView();
        }
    }
}