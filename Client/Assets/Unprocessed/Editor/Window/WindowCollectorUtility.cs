using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorModule
{
    public class WindowCollectorUtilityConfig
    {
        /// 隐藏目录
        public bool HideList;
        /// 隐藏通用模块
        public bool HideUtility = true;
        /// 隐藏底部模块
        public bool HideBottom = true;
    }
        
    public interface IWindowCollectorUtility
    {
        void OnGUI(Action insert);

        WindowCollectorUtilityConfig GetConfig();
    }

    public partial class WindowCollectorUtility : IWindowCollectorUtility
    {
        private WindowCollectorUtilityConfig config
            = new WindowCollectorUtilityConfig();

        void IWindowCollectorUtility.OnGUI(Action insert)
        {
            // 顶部模块
            TopModule();

            // 通用模块
            if (!config.HideUtility) UtilityModule();
            
            // 中间模块
            insert.Invoke();

            // 底部模块
            if (!config.HideBottom)
            {
                GUILayout.FlexibleSpace();
                BottomModule();
            }
        }

        WindowCollectorUtilityConfig IWindowCollectorUtility.GetConfig() => config;

        /// 顶部模块
        private void TopModule()
        {
            using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
            {
                if (GUILayout.Button(config.HideList ? "显示目录" : "隐藏目录"))
                {
                    config.HideList = !config.HideList;
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button(config.HideUtility ? "显示通用模块" : "隐藏通用模块"))
                {
                    config.HideUtility = !config.HideUtility;
                }
                if (GUILayout.Button(config.HideBottom ? "显示底部模块" : "隐藏底部模块"))
                {
                    config.HideBottom = !config.HideBottom;
                }
            }
        }

        /// 通用模块
        private void UtilityModule()
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                // 按钮列表
                {
                    int lineBreak = 5;
                    var btnList = BtnList.ToList();
                    for (int i = 0; i + lineBreak < btnList.Count; i+= lineBreak)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            for (int j = i; j < i + lineBreak; j++)
                            {
                                var pair = btnList[j];
                                if (GUILayout.Button(pair.Key))
                                {
                                    pair.Value.Invoke();
                                }
                            }
                        }
                    }
                    if (btnList.Count % lineBreak != 0)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            for (int i = btnList.Count - btnList.Count % lineBreak; i < btnList.Count; i++)
                            {
                                var pair = btnList[i];
                                if (GUILayout.Button(pair.Key))
                                {
                                    pair.Value.Invoke();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// 底部模块
        private void BottomModule()
        {
            using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("底部模块");
            }
        }
    }
}
