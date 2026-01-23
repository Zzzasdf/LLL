using System;
using System.Collections.Generic;

public class PopupContainer: ILayerContainer
{
    private readonly int warmCapacity;
    
    private HashSet<int> uniqueIds;
    private Dictionary<int, Queue<int>> storageDict;

    private ILayerLocator layerLocator;
    private Func<int, IView> getViewFunc;

    public PopupContainer(int warmCapacity)
    {
        this.warmCapacity = warmCapacity;
        uniqueIds = new HashSet<int>();
        storageDict = new Dictionary<int, Queue<int>>();
    }

    void ILayerContainer.BindLocator(ILayerLocator layerLocator) => this.layerLocator = layerLocator;
    ILayerLocator ILayerContainer.GetLocator() => layerLocator;
    void ILayerContainer.BindGetView(Func<int, IView> getViewFunc) => this.getViewFunc = getViewFunc;

    bool ILayerContainer.AddAndTryOutRemoveId(int uniqueId, out int removeId)
    {
        removeId = 0;
        if (uniqueIds.Count == warmCapacity)
        {
            LLogger.FrameWarning($"{nameof(StackContainer)} 的容量已超过预警值：{warmCapacity}");
        }
        uniqueIds.Add(uniqueId);
        return false;
    }
    bool ILayerContainer.RemoveAndTryPopId(int uniqueId, out int popId)
    {
        popId = 0;
        uniqueIds.Remove(uniqueId);
        return false;
    }
    
    void ILayerContainer.PushStorage(int uniqueId)
    {
        if (uniqueIds.Count == 0) return;
        if (!storageDict.TryGetValue(uniqueId, out Queue<int> stack))
        {
            storageDict.Add(uniqueId, stack = new Queue<int>());
        }
        foreach (int id in uniqueIds)
        {
            IView view = getViewFunc.Invoke(id);
            view.Hide();
            stack.Enqueue(id);
        }
        uniqueIds.Clear();
    }
    bool ILayerContainer.TryPopStorage(int uniqueId, out Queue<int> storage)
    {
        return storageDict.Remove(uniqueId, out storage);
    }
}
