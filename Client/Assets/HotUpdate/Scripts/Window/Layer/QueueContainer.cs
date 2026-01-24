using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

public class QueueContainer : ILayerContainer
{
    private readonly LayerContainerAssets layerContainerAssets;
    private readonly int capacity;
    
    private LinkedList<int> uniqueIds;
    private Dictionary<int, Queue<int>> stashDict;

    public QueueContainer(ViewLayer viewLayer, int capacity)
    {
        layerContainerAssets = new LayerContainerAssets(viewLayer, isMultiple: false);
        this.capacity = capacity;
        uniqueIds = new LinkedList<int>();
        stashDict = new Dictionary<int, Queue<int>>();
    }
    
    void ILayerContainer.BindLocator(ILayerLocator layerLocator) => layerContainerAssets.BindLocator(layerLocator);
    
    UniTask<IView> ILayerContainer.ShowViewAsync(Type type) => layerContainerAssets.ShowViewAsync(type);
    void ILayerContainer.HideView(int uniqueId) => layerContainerAssets.HideView(uniqueId);

    bool ILayerContainer.TryPopView(int uniqueId) => layerContainerAssets.TryPopView(uniqueId);

    
    void ILayerContainer.HideAllView()
    {
        while (uniqueIds.Count != 0)
        {
            int uniqueId = uniqueIds.First();
            ILayerContainer layerContainer = this;
            layerContainer.PopAndTryPush(uniqueId, out _);
            layerContainer.HideView(uniqueId);
        }
        stashDict.Clear();
    }
    
    bool ILayerContainer.PushAndTryPop(int uniqueId, out int popId)
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
    bool ILayerContainer.PopAndTryPush(int uniqueId, out int pushId)
    {
        pushId = 0;
        uniqueIds.RemoveFirst();
        return false;
    }
    
    void ILayerContainer.StashPush(int uniqueId)
    {
        if (uniqueIds.Count == 0) return;
        if (!stashDict.TryGetValue(uniqueId, out Queue<int> queue))
        {
            stashDict.Add(uniqueId, queue = new Queue<int>());
        }
        foreach (int id in uniqueIds)
        { 
            IView view = layerContainerAssets.GetView(id);
            view.Hide();
            queue.Enqueue(id);
        }
        uniqueIds.Clear();
    }
    bool ILayerContainer.TryStashPop(int uniqueId, out Queue<int> popIds)
    {
        return stashDict.Remove(uniqueId, out popIds);
    }
}
