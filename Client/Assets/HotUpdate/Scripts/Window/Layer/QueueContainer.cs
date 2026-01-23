using System;
using System.Collections.Generic;

public class QueueContainer : ILayerContainer
{
    private readonly int capacity;
    
    private LinkedList<int> uniqueIds;
    private Dictionary<int, Queue<int>> storageDict;

    private ILayerLocator layerLocator;
    private Func<int, IView> getViewFunc;

    public QueueContainer(int capacity)
    {
        this.capacity = capacity;
        uniqueIds = new LinkedList<int>();
        storageDict = new Dictionary<int, Queue<int>>();
    }

    void ILayerContainer.BindLocator(ILayerLocator layerLocator) => this.layerLocator = layerLocator;
    ILayerLocator ILayerContainer.GetLocator() => layerLocator;
    void ILayerContainer.BindGetView(Func<int, IView> getViewFunc) => this.getViewFunc = getViewFunc;

    bool ILayerContainer.AddAndTryOutRemoveId(int uniqueId, out int removeId)
    {
        bool result = false;
        removeId = 0;
        if (uniqueIds.Count == capacity)
        {
            removeId = uniqueIds.First.Value;
            uniqueIds.RemoveFirst();
            result = true;
        }
        uniqueIds.AddLast(uniqueId);
        return result;
    }
    bool ILayerContainer.RemoveAndTryPopId(int uniqueId, out int popId)
    {
        popId = 0;
        uniqueIds.RemoveFirst();
        return false;
    }
    
    void ILayerContainer.PushStorage(int uniqueId)
    {
        if (uniqueIds.Count == 0) return;
        if (!storageDict.TryGetValue(uniqueId, out Queue<int> queue))
        {
            storageDict.Add(uniqueId, queue = new Queue<int>());
        }
        foreach (int id in uniqueIds)
        { 
            IView view = getViewFunc.Invoke(id);
            view.Hide();
            queue.Enqueue(id);
        }
        uniqueIds.Clear();
    }
    bool ILayerContainer.TryPopStorage(int uniqueId, out Queue<int> storage)
    {
        return storageDict.Remove(uniqueId, out storage);
    }
}
