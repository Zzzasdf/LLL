using System.Collections.Generic;
using System.Linq;

public class QueueContainer : SingleLayerContainerBase
{
    private readonly int capacity;
    
    private LinkedList<int> uniqueIds;
    private Dictionary<int, Queue<int>> stashDict;

    public QueueContainer(int capacity): base()
    {
        this.capacity = capacity;
        uniqueIds = new LinkedList<int>();
        stashDict = new Dictionary<int, Queue<int>>();
    }
    
    public override void HideAllView()
    {
        while (uniqueIds.Count != 0)
        {
            int uniqueId = uniqueIds.First();
            PopAndTryPush(uniqueId, out _);
            HideView(uniqueId);
        }
        stashDict.Clear();
    }
    
    public override bool PushAndTryPop(int uniqueId, out int popId)
    {
        bool result = false;
        popId = 0;
        if (uniqueIds.Count == capacity)
        {
            popId = uniqueIds.First.Value;
            uniqueIds.RemoveFirst();
            result = true;
        }
        uniqueIds.AddLast(uniqueId);
        return result;
    }
    public override bool PopAndTryPush(int uniqueId, out int pushId)
    {
        pushId = 0;
        uniqueIds.RemoveFirst();
        return false;
    }
    
    public override void StashPush(int uniqueId)
    {
        if (uniqueIds.Count == 0) return;
        if (!stashDict.TryGetValue(uniqueId, out Queue<int> queue))
        {
            stashDict.Add(uniqueId, queue = new Queue<int>());
        }
        foreach (int id in uniqueIds)
        { 
            IView view = uniqueViewDict[id];
            view.Hide();
            queue.Enqueue(id);
        }
        uniqueIds.Clear();
    }
    public override bool TryStashPop(int uniqueId, out Queue<int> popIds)
    {
        return stashDict.Remove(uniqueId, out popIds);
    }
}
