using UnityEngine;

namespace Editor.StateMachineModule 
{
    public class StateMachineEditorWindowRRRR : StateMachineEditorWindow<StateMachineDataRRRR, StateNodeRRRR, TransitionRRRR>
    {
        
    }

    [CreateAssetMenu(fileName = "NewStateMachineDataRRRR", menuName = "State Machine Data/RRRR")]
    public class StateMachineDataRRRR : StateMachineData<StateNodeRRRR, TransitionRRRR>
    {
    
    }

    [System.Serializable]
    public class StateNodeRRRR : StateNode
    {
    
    }

    [System.Serializable]
    public class TransitionRRRR : Transition 
    {
    
    }
}
