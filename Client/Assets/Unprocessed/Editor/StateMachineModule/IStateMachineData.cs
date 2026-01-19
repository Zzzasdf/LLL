using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface IStateMachine
{
    void Init(string folderRootPath, List<FileInfo> stateMachineDataFileInfos, Action repaintCallback);
    void Repaint();
    void OnGUI();
    Type BindDataType();
}

public interface IStateMachineData 
{
    
}

public interface IStateMachineData<TStateNode, TTransition>: IStateMachineData 
    where TStateNode: IStateNode
    where TTransition: ITransition
{
    List<TStateNode> States { get; set; }
    List<TTransition> Transitions  { get; set; }

    TStateNode GetStateByUniqueId(int uniqueId);
    int UniqueIdGenerator();
    bool ContainsUniqueId(int uniqueId);
    string UniqueNameGenerator();
    bool ContainsName(string name);
    void Export();
}

public interface IStateNode: IElement
{
    int UniqueId { get; set; }
    string Name { get; set; }
    ENode ENode { get; set; }
    Rect Rect { get; set; } // 节点在窗口中的位置
    bool IsEntryState { get; set; } // 是否是入口状态

    Vector2 AdaptedSize();
    int DrawBoxElement();
}

public interface ITransition: IElement
{
    int FromUniqueId { get; set; }
    int ToUniqueId { get; set; }
}

public interface IElement 
{
    void DrawInspector();
}
