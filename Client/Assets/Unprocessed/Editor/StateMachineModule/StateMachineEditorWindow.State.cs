using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.StateMachineModule 
{
    public partial class StateMachineEditorWindow<TStateMachineData, TStateNode, TTransition>
    {
        /// 绘制所有状态节点
        private void DrawStateNodes()
        {
            if (selectedStateMachineData == null) return;
            foreach (var state in selectedStateMachineData.States)
            {
                DrawStateNode(state);
            }
        }
        /// 绘制状态节点
        private void DrawStateNode(TStateNode state)
        {
            state.DrawBoxElement();
            // 允许拖拽节点
            if (Event.current.type == EventType.MouseDrag && state.Rect.Contains(Event.current.mousePosition))
            {
                Rect rect = state.Rect;
                rect.position += Event.current.delta;
                state.Rect = rect;
                ((IStateMachine)this).Repaint();
            }
        }

#region 添加 State
        private void AddState(Vector2 position, ENode eNode)
        {
            if (selectedStateMachineData == null)
            {
                Debug.LogError("StateMachineData is null!");
                return;
            }
            if (selectedStateMachineData.States == null)
            {
                selectedStateMachineData.States = new List<TStateNode>();
            }
            if (selectedStateMachineData.States.Count == 0)
            {
                eNode = ENode.Branch;
            }
            TStateNode newState = new TStateNode()
            {
                UniqueId = selectedStateMachineData.UniqueIdGenerator(),
                ENode = eNode,
                Name = selectedStateMachineData.UniqueNameGenerator(),
                IsEntryState = selectedStateMachineData.States.Count == 0,
            };
            newState.Rect = new Rect(position, newState.AdaptedSize());

            selectedStateMachineData.States.Add(newState);
            EditorUtility.SetDirty(selectedStateMachineData);
            AssetDatabase.SaveAssets();
            ((IStateMachine)this).Repaint();
        }
#endregion

#region 移除 State
        private void DeleteStateNode(TStateNode nodeToDelete)
        {
            if (selectedStateMachineData == null || nodeToDelete == null) return;
    
            // 1. 从状态列表中移除节点
            selectedStateMachineData.States.Remove(nodeToDelete);
    
            // 2. 清理所有相关过渡
            selectedStateMachineData.Transitions.RemoveAll(t => 
                t.FromUniqueId == nodeToDelete.UniqueId || t.ToUniqueId == nodeToDelete.UniqueId);
    
            // 3. 处理入口状态
            if (nodeToDelete.IsEntryState && selectedStateMachineData.States.Count > 0)
            {
                // 自动设置第一个节点为新的入口
                selectedStateMachineData.States[0].ENode = ENode.Branch;
                selectedStateMachineData.States[0].IsEntryState = true;
            }
    
            // 4. 保存更改
            EditorUtility.SetDirty(selectedStateMachineData);
            AssetDatabase.SaveAssets();
            ((IStateMachine)this).Repaint();
        }
#endregion

#region 详情 State
        private void DetailStateNode(TStateNode selectedNode)
        {
            selectedElement = selectedNode;
        }
#endregion

        private bool TryGetStateNodeAtPosition(Vector2 position, out IElement result)
        {
            result = null;
            if (selectedStateMachineData?.Transitions == null) return false;
            
            foreach (var state in selectedStateMachineData.States)
            {
                if (state.Rect.Contains(position))
                {
                    result = state;
                    return true;
                }
            }
            return false;
        }
    }
}
