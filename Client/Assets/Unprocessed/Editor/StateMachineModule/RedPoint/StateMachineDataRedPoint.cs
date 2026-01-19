using UnityEditor;
using UnityEngine;

namespace Editor.StateMachineModule 
{
    public class StateMachineEditorWindowRedPoint : StateMachineEditorWindow<StateMachineDataRedPoint, StateNodeRedPoint, TransitionRedPoint>
    {
        
    }

    [CreateAssetMenu(fileName = "NewStateMachineDataRedPoint", menuName = "State Machine Data/RedPoint")]
    public class StateMachineDataRedPoint : StateMachineData<StateNodeRedPoint, TransitionRedPoint>
    {
    
    }

    [System.Serializable]
    public class StateNodeRedPoint : StateNode 
    {
        public ERedPoint eRedPoint;

        public override Vector2 AdaptedSize() => new Vector2(100, 40);
        public override int DrawBoxElement()
        {
            int paintedCount = base.DrawBoxElement();
            float startY = rect.position.y - rect.size.y / 2 + 20 * (paintedCount + 1);
            Vector2 position = new Vector2(rect.position.x, startY);
            GUI.Box(new Rect(position, new Vector2(100, 20)), eRedPoint.ToString()); //, style))
            return 2;
        }
        
        public override void DrawInspector()
        {
            base.DrawInspector();
            eRedPoint = (ERedPoint)EditorGUILayout.EnumPopup("eRedPoint", eRedPoint, EditorStyles.popup);
        }

    }

    [System.Serializable]
    public class TransitionRedPoint : Transition 
    {
    
    }
}
