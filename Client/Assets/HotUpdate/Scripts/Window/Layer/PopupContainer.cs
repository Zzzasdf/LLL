using System.Collections.Generic;
using System.Linq;

public class PopupContainer: MultiLayerContainerBase
{
    private readonly int warmCapacity;
    
    private HashSet<int> uniqueIds;
    private Dictionary<int, Queue<int>> stashDict;

    public PopupContainer(int warmCapacity): base()
    {
        this.warmCapacity = warmCapacity;
        uniqueIds = new HashSet<int>();
        stashDict = new Dictionary<int, Queue<int>>();
    }

    public override void HideAllView()
    {
        List<int> uniqueIds = this.uniqueIds.ToList();
        foreach (var uniqueId in uniqueIds)
        {
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
        uniqueIds.Add(uniqueId);
        return false;
    }
    public override bool PopAndTryPush(int uniqueId, out int pushId)
    {
        pushId = 0;
        uniqueIds.Remove(uniqueId);
        return false;
    }
    
    public override void StashPush(int uniqueId)
    {
        if (uniqueIds.Count == 0) return;
        if (!stashDict.TryGetValue(uniqueId, out Queue<int> stack))
        {
            stashDict.Add(uniqueId, stack = new Queue<int>());
        }
        foreach (int id in uniqueIds)
        {
            IView view = uniqueViewDict[id];
            view.Hide();
            stack.Enqueue(id);
        }
        uniqueIds.Clear();
    }
    public override bool TryStashPop(int uniqueId, out Queue<int> popIds)
    {
        return stashDict.Remove(uniqueId, out popIds);
    }
}
