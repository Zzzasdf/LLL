using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor.StateMachineModule
{
    public partial class StateMachineEditorWindow<TStateMachineData, TStateNode, TTransition>
    {
        /// 顶部工具栏
        private void DrawToolbar()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    int changeIndex = EditorGUILayout.Popup(inRefIndex, displayName);
                    if (changeIndex != inRefIndex)
                    {
                        inRefIndex = changeIndex;
                        FileInfo fileInfo = stateMachineDataFileInfos[inRefIndex];
                        string path = fileInfo.FullName[fileInfo.FullName.IndexOf("Assets", StringComparison.Ordinal)..];
                        selectedStateMachineData = AssetDatabase.LoadAssetAtPath<TStateMachineData>(path);
                    }
                    if (GUILayout.Button("Export"))
                    {
                        if (selectedStateMachineData != null)
                        {
                            selectedStateMachineData.Export();
                        }
                    }
                }
            }
        }
        
#region 绘制 Inspector
        private IElement _selectedElement;
        private IElement selectedElement 
        {
            get => _selectedElement;
            set 
            {
                GUI.FocusControl(null);
                _selectedElement = value;
            }
        }
        private void DrawInspector()
        {
            if (selectedStateMachineData == null) return;
            selectedElement?.DrawInspector();
        }
#endregion   

#region 绘制 空点击 菜单
        private void DrawNullClickMenu()
        {
            Vector2 mousePos = Event.current.mousePosition;
            GenericMenu menu = new GenericMenu();
            // 确保 stateMachineData 存在
            if (selectedStateMachineData == null)
            {
                menu.AddDisabledItem(new GUIContent("Load a StateMachineData first!"));
            }
            else
            {
                if (selectedStateMachineData.States.Count == 0)
                {
                    menu.AddItem(new GUIContent("Add Entry"), false, () => AddState(mousePos, ENode.Branch));
                }
                else
                {
                    menu.AddItem(new GUIContent("Add State"), false, () => AddState(mousePos, ENode.Normal));
                    menu.AddItem(new GUIContent("Add Branch"), false, () => AddState(mousePos, ENode.Branch));
                }
            }
            menu.ShowAsContext();
        }
#endregion

#region 绘制选中 State 菜单
        private void DrawSelectedStateMenu(TStateNode selectedNode)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Detail State"), false, () => DetailStateNode(selectedNode));
            menu.AddItem(new GUIContent("Delete State"), false, () => DeleteStateNode(selectedNode));
            menu.AddItem(new GUIContent("Add Transition"), false, () => StartAddTransition(selectedNode));
            menu.ShowAsContext();
        }
#endregion

#region 绘制选中 Transition 菜单
        private void DrawSelectedTransitionMenu(TTransition selectedTransition)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete Transition"), false, () => DeleteTransition(selectedTransition));
            menu.ShowAsContext();
        }
#endregion
    }
}