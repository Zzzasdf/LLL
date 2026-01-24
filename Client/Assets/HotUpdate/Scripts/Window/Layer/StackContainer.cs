using System.Collections.Generic;

public class StackContainer: SingleLayerContainerBase
{
    private readonly int warmCapacity;
    
    private Stack<int> uniqueIds;
    private Dictionary<int, Stack<int>> stashDict;

    public StackContainer(int warmCapacity): base()
    {
        this.warmCapacity = warmCapacity;
        uniqueIds = new Stack<int>();
        stashDict = new Dictionary<int, Stack<int>>();
    }
    
    public override void HideAllView()
    {
        while (uniqueIds.Count != 0)
        {
            int uniqueId = uniqueIds.Pop();
            PopAndTryPush(uniqueId, out _);
            HideView(uniqueId);
        }
        stashDict.Clear();
    }
    
    public override bool PushAndTryPop(int uniqueId, out int popId)
    {
        popId = 0;
        if (uniqueIds.Count == warmCapacity)
        {
            LLogger.FrameWarning($"{nameof(StackContainer)} 的容量已超过预警值：{warmCapacity}");
        }
        if (uniqueIds.Count > 0)
        {
            int hideId = uniqueIds.Peek();
            IView view = uniqueViewDict[hideId];
            view.Hide();
        }
        uniqueIds.Push(uniqueId);
        return false;
    }
    public override bool PopAndTryPush(int uniqueId, out int pushId)
    {
        pushId = 0;
        if (uniqueIds.Count == 0
            || uniqueIds.Peek() != uniqueId)
        {
            return false;
        }
        uniqueIds.Pop();
        if (uniqueIds.Count == 0)
        {
            return false;
        }
        pushId = uniqueIds.Pop();
        return true;
    }
    
    public override void StashPush(int uniqueId)
    {
        if (uniqueIds.Count == 0) return;
        if (!stashDict.TryGetValue(uniqueId, out Stack<int> stack))
        {
            stashDict.Add(uniqueId, stack = new Stack<int>());
        }
        foreach (int id in uniqueIds)
        {
            IView view = uniqueViewDict[id];
            view.Hide();
            stack.Push(id);
        }
        uniqueIds.Clear();
    }
    public override bool TryStashPop(int uniqueId, out Queue<int> popIds)
    {
        popIds = null;
        if (!stashDict.Remove(uniqueId, out Stack<int> stack))
        {
            return false;
        }
        popIds = new Queue<int>();
        foreach (int id in stack)
        {
            popIds.Enqueue(id);
        }
        return true;
    }
}
