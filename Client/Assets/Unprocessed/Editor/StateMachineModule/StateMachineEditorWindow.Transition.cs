using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.StateMachineModule
{
    public partial class StateMachineEditorWindow<TStateMachineData, TStateNode, TTransition>
    {
        private float curveOffset = 0f;//50f;
        private Color transitionColor = Color.green;

        private float arrowSize = 20f;
        private Color arrowColor = Color.red;
        
        /// 绘制过渡曲线
        private void DrawTransitions()
        {
            if (selectedStateMachineData == null) return;
            Handles.BeginGUI();
            foreach (var transition in selectedStateMachineData.Transitions)
            {
                TStateNode fromState = selectedStateMachineData.States.Find(s => s.UniqueId == transition.FromUniqueId);
                TStateNode toState = selectedStateMachineData.States.Find(s => s.UniqueId == transition.ToUniqueId);

                if (fromState != null && toState != null)
                {
                   DrawBezier(fromState, toState);
                }
            }
            Handles.EndGUI();
        }
        /// 绘制贝塞尔曲线
        private void DrawBezier(TStateNode fromState, TStateNode toState)
        {
            Vector2 start = fromState.Rect.center;
            start.y -= fromState.Rect.height / 2;
            Vector2 end = toState.Rect.center;
            end.y += toState.Rect.height / 2;
                    
            // 计算控制点（使曲线呈弧形）
            Vector2 startTangent = start + Vector2.right * curveOffset;
            Vector2 endTangent = end - Vector2.right * curveOffset;
            using (new Handles.DrawingScope(transitionColor))
            {
                Handles.DrawBezier(start, end, startTangent, endTangent, transitionColor, null, 5);
            }
                    
            // 绘制条件标签
            Vector2 labelPos = (start + end) * 0.5f;
            Handles.Label(labelPos, "11111", EditorStyles.miniLabel);
                    
            // 计算箭头位置和方向
            Vector2 arrowDir = ((start - end) * 0.5f).normalized;
            Vector2 arrowBase = (end + start) * 0.5f - arrowDir * (arrowSize * 0.5f); // 箭头起点略微向内偏移

            // 绘制箭头
            DrawArrowHead(arrowBase, arrowDir, arrowSize, arrowColor);
        }
        // 绘制箭头（三角形）
        private void DrawArrowHead(Vector2 pos, Vector2 direction, float size, Color color)
        {
            Vector2 right = Quaternion.Euler(0, 0, 30) * direction * size;
            Vector2 left = Quaternion.Euler(0, 0, -30) * direction * size;
            using (new Handles.DrawingScope(color))
            {
                Handles.DrawAAConvexPolygon(pos, pos + right, pos + left);
            }
        }

#region 添加 Transition
        private TStateNode dragStartNode; // 记录拖拽起始节点
        private void StartAddTransition(TStateNode selectedNode)
        {
            dragStartNode = selectedNode;
            eEventStatus = EEventStatus.AddTransition;
        }
        private void EndAddTransition(TStateNode selectedNode)
        {
            if (selectedNode != null)
            {
                if (selectedNode != dragStartNode)
                {
                    AddTransition(dragStartNode, selectedNode);
                }
            }
            dragStartNode = null;
            eEventStatus = EEventStatus.Normal;
        }
        private void AddTransition(TStateNode fromState, TStateNode toState)
        {
            if (selectedStateMachineData.Transitions == null)
            {
                selectedStateMachineData.Transitions = new List<TTransition>();
            }
            if (selectedStateMachineData.Transitions.Exists(t => 
                t.FromUniqueId == fromState.UniqueId && t.ToUniqueId == toState.UniqueId))
            {
                Debug.LogWarning("Transition already exists!");
                return;
            }

            var newTransition = new TTransition()
            {
                FromUniqueId = fromState.UniqueId,
                ToUniqueId = toState.UniqueId,
            };

            selectedStateMachineData.Transitions.Add(newTransition);
            EditorUtility.SetDirty(selectedStateMachineData);
            AssetDatabase.SaveAssets();
            ((IStateMachine)this).Repaint();
        }
#endregion

#region 删除 Transition
        private void DeleteTransition(TTransition selectedTransition)
        {
            selectedStateMachineData.Transitions.Remove(selectedTransition);
            EditorUtility.SetDirty(selectedStateMachineData);
            AssetDatabase.SaveAssets();
            ((IStateMachine)this).Repaint();
        }
#endregion
        
        private bool TryGetTransitionAtPosition(Vector2 position, out IElement result)
        {
            result = null;
            if (selectedStateMachineData?.Transitions == null) return false;

            foreach (var transition in selectedStateMachineData.Transitions)
            {
                TStateNode fromState = selectedStateMachineData.GetStateByUniqueId(transition.FromUniqueId);
                TStateNode toState = selectedStateMachineData.GetStateByUniqueId(transition.ToUniqueId);
        
                if (fromState == null || toState == null) continue;
        
                // 计算控制点（根据节点位置自动调整曲线方向）
                Vector2 start = fromState.Rect.center;
                start.y -= fromState.Rect.height / 2;
                Vector2 end = toState.Rect.center;
                end.y += toState.Rect.height / 2;
                    
                // // 计算控制点（使曲线呈弧形）
                // Vector2 startTangent = start + Vector2.right * curveOffset;
                // Vector2 endTangent = end - Vector2.right * curveOffset;
        
                // 计算箭头位置和方向
                Vector2 arrowDir = ((start - end) * 0.5f).normalized;
                Vector2 arrowBase = (end + start) * 0.5f - arrowDir * (arrowSize * 0.5f); // 箭头起点略微向内偏移
                
                Rect rect = new Rect(arrowBase.x- arrowSize, arrowBase.y - arrowSize, arrowSize * 2f, arrowSize * 2f);
                // GUIStyle style = new GUIStyle(GUI.skin.box);
                // style.normal.background = MakeTex(2, 2,Color.black);
                // GUI.Box(rect, "AAA", style);
                if (rect.Contains(position))
                {
                    result = transition;
                    return true;
                }

                // // 检测点是否在曲线附近
                // if (BezierUtils.IsPointNearBezier(
                //     start, 
                //     end, 
                //     startTangent, 
                //     endTangent, 
                //     position, 
                //     10f)) // 检测半径
                // {
                //     result = transition;
                //     return true;
                // }
            }
            return false;
        }
    }
}