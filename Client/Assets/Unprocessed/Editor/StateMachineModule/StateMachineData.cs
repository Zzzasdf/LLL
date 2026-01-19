using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class StateMachineData<TStateNode, TTransition> : ScriptableObject, IStateMachineData<TStateNode, TTransition>
    where TStateNode: class, IStateNode, new()
    where TTransition: class, ITransition, new()
{
    public List<TStateNode> states = new List<TStateNode>();
    public List<TTransition> transitions = new List<TTransition>();
    
    List<TStateNode> IStateMachineData<TStateNode, TTransition>.States 
    {
        get => states;
        set => states = value;
    }
    List<TTransition> IStateMachineData<TStateNode, TTransition>.Transitions 
    {
        get => transitions;
        set => transitions = value;
    }
    
    /// 唯一 id 查找 node
    TStateNode IStateMachineData<TStateNode, TTransition>.GetStateByUniqueId(int uniqueId)
    {
        foreach (var state in states)
        {
            if (state.UniqueId != uniqueId) continue;
            return state;
        }
        return null;
    }

    /// 唯一 id 生成器
    int IStateMachineData<TStateNode, TTransition>.UniqueIdGenerator()
    {
        IStateMachineData<TStateNode, TTransition> data = this;
        int result = 0;
        while (data.ContainsUniqueId(result))
        {
            result++;
        }
        return result;
    }
    /// 是否 包含 唯一 id
    bool IStateMachineData<TStateNode, TTransition>.ContainsUniqueId(int uniqueId)
    {
        return states.Any(x => x.UniqueId == uniqueId);
    }

    /// 唯一 名称 生成器
    string IStateMachineData<TStateNode, TTransition>.UniqueNameGenerator()
    {
        IStateMachineData<TStateNode, TTransition> data = this;
        string result;
        int i = 0;
        do
        {
            result = string.Format($"New State ({i++})");
        } while (data.ContainsName(result));
        return result;
    }
    /// 是否 包含 名称
    bool IStateMachineData<TStateNode, TTransition>.ContainsName(string name)
    {
        return states.Any(x => x.Name == name);
    }

    void IStateMachineData<TStateNode, TTransition>.Export()
    {
        Debug.LogError("导出");
    }
}

[System.Serializable]
public class StateNode: IStateNode
{
    public int uniqueId;
    public string name;
    public ENode eNode;
    public Rect rect; // 节点在窗口中的位置
    public bool isEntryState; // 是否是入口状态
    
    int IStateNode.UniqueId 
    {
        get => uniqueId;
        set => uniqueId = value;
    }
    string IStateNode.Name 
    {
        get => name;
        set => name = value;
    }
    ENode IStateNode.ENode 
    {
        get => eNode;
        set => eNode = value;
    }
    Rect IStateNode.Rect 
    {
        get => rect;
        set => rect = value;
    }
    bool IStateNode.IsEntryState 
    {
        get => isEntryState;
        set => isEntryState = value;
    }

    public virtual Vector2 AdaptedSize() => new Vector2(100, 20);
    public virtual int DrawBoxElement()
    {
        GUIStyle style = new GUIStyle(GUI.skin.box);
        Color bgColor = isEntryState ? new Color(100 / 255f, 100 / 255f, 160 / 255f) : Color.gray;
        style.normal.background = MakeTex(2, 2, bgColor);
        if(eNode == ENode.Branch)
        {
            float scaleAdapte = 1.5f;
            Vector2 center = rect.center;
            Vector3[] diamondPoints = new Vector3[4]
            {
                center + new Vector2(0, -rect.height * 0.5f * scaleAdapte), // 上
                center + new Vector2(rect.width * 0.5f * scaleAdapte, 0),  // 右
                center + new Vector2(0, rect.height * 0.5f * scaleAdapte),  // 下
                center + new Vector2(-rect.width * 0.5f * scaleAdapte, 0), // 左
            };
            // 绘制菱形背景
            Handles.BeginGUI();
            Handles.color = bgColor;
            Handles.DrawAAConvexPolygon(diamondPoints);
            Handles.EndGUI();
        }
        GUI.Box(rect, "", style);
        
        float startY = rect.position.y - rect.size.y / 2 + 10;
        Vector2 position = new Vector2(rect.position.x, startY);
        GUI.Box(new Rect(position, new Vector2(100, 20)), name, style);
        return 1;
    }
    
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
    
    public virtual void DrawInspector()
    {
        EditorGUILayout.LabelField("StateNode");
        GUI.enabled = false;
        EditorGUILayout.IntField("uniqueId", uniqueId);
        EditorGUILayout.EnumPopup("eNode", eNode, EditorStyles.popup);
        GUI.enabled = true;
        name = EditorGUILayout.TextField("name", name);
    }
}
public enum ENode 
{
    Normal,
    Branch,
}

[System.Serializable]
public class Transition: ITransition
{
    public int fromUniqueId;
    public int toUniqueId;
    
    int ITransition.FromUniqueId 
    {
        get => fromUniqueId;
        set => fromUniqueId = value;
    }
    int ITransition.ToUniqueId 
    {
        get => toUniqueId;
        set => toUniqueId = value;
    }

    public virtual void DrawInspector()
    {
        EditorGUILayout.LabelField("Transition");
        GUI.enabled = false;
        EditorGUILayout.IntField("fromUniqueId", fromUniqueId);
        EditorGUILayout.IntField("toUniqueId", toUniqueId);
        GUI.enabled = true;
    }
}
