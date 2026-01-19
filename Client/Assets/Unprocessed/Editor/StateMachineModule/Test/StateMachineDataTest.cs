using UnityEngine;

namespace Editor.StateMachineModule
{
    public class StateMachineEditorWindowTest : StateMachineEditorWindow<StateMachineDataTest, StateNodeTest, TransitionTest>
    {
        
    }

    [CreateAssetMenu(fileName = "NewStateMachineDataTest", menuName = "State Machine Data/Test")]
    public class StateMachineDataTest : StateMachineData<StateNodeTest, TransitionTest>
    {
    
    }

    [System.Serializable]
    public class StateNodeTest : StateNode
    {
    
    }

    [System.Serializable]
    public class TransitionTest : Transition 
    {
    
    }
}
