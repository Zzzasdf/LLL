using System;
using System.Collections.Generic;

public class StackContainer: ILayerContainer
{
    private readonly int warmCapacity;
    
    private Stack<int> uniqueIds;
    private Dictionary<int, Stack<int>> storageDict;

    private ILayerLocator layerLocator;
    private Func<int, IView> getViewFunc;
    
    public StackContainer(int warmCapacity)
    {
        this.warmCapacity = warmCapacity;
        uniqueIds = new Stack<int>();
        storageDict = new Dictionary<int, Stack<int>>();
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
        if (uniqueIds.Count > 0)
        {
            int hideId = uniqueIds.Peek();
            IView view = getViewFunc.Invoke(hideId);
            view.Hide();
        }
        uniqueIds.Push(uniqueId);
        return false;
    }
    bool ILayerContainer.RemoveAndTryPopId(int uniqueId, out int popId)
    {
        popId = 0;
        if (uniqueIds.Peek() != uniqueId)
        {
            return false;
        }
        uniqueIds.Pop();
        if (uniqueIds.Count == 0)
        {
            return false;
        }
        popId = uniqueIds.Pop();
        return true;
    }
    
    void ILayerContainer.PushStorage(int uniqueId)
    {
        if (uniqueIds.Count == 0) return;
        if (!storageDict.TryGetValue(uniqueId, out Stack<int> stack))
        {
            storageDict.Add(uniqueId, stack = new Stack<int>());
        }
        foreach (int id in uniqueIds)
        {
            IView view = getViewFunc.Invoke(id);
            view.Hide();
            stack.Push(id);
        }
        uniqueIds.Clear();
    }
    bool ILayerContainer.TryPopStorage(int uniqueId, out Queue<int> storage)
    {
        storage = null;
        if (!storageDict.Remove(uniqueId, out Stack<int> stack))
        {
            return false;
        }
        storage = new Queue<int>();
        foreach (int id in stack)
        {
            storage.Enqueue(id);
        }
        return true;
    }
}
