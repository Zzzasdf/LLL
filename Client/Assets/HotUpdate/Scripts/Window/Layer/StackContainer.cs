using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class StackContainer: ILayerContainer
{
    private readonly LayerContainerAssets layerContainerAssets;
    private readonly int warmCapacity;
    
    private Stack<int> uniqueIds;
    private Dictionary<int, Stack<int>> stashDict;

    public StackContainer(ViewLayer viewLayer, int warmCapacity)
    {
        layerContainerAssets = new LayerContainerAssets(viewLayer, isMultiple: false);
        this.warmCapacity = warmCapacity;
        uniqueIds = new Stack<int>();
        stashDict = new Dictionary<int, Stack<int>>();
    }

    void ILayerContainer.BindLocator(ILayerLocator layerLocator) => layerContainerAssets.BindLocator(layerLocator);

    async UniTask<(IView view, int? removeId)> ILayerContainer.ShowViewAndTryRemoveAsync(Type type)
    {
        IView view = await layerContainerAssets.ShowViewAsync(type);
        int uniqueId = view.GetUniqueId();
        PushAndTryPop(uniqueId);
        return (view, null);
    }
    int? ILayerContainer.PopViewAndTryRemove(int uniqueId)
    {
        if (!layerContainerAssets.TryPopView(uniqueId))
        {
            return null;
        }
        PushAndTryPop(uniqueId);
        return null;
    }
    private void PushAndTryPop(int uniqueId)
    {
        if (uniqueIds.Count == warmCapacity)
        {
            LLogger.FrameWarning($"{nameof(StackContainer)} 的容量已超过预警值：{warmCapacity}");
        }
        if (uniqueIds.Count > 0)
        {
            int hideId = uniqueIds.Peek();
            IView view = layerContainerAssets.GetView(hideId);
            view.Hide();
        }
        uniqueIds.Push(uniqueId);
    }

    int? ILayerContainer.HideViewTryPop(int uniqueId)
    {
        int? popId = RemoveAndTryPop(uniqueId);
        layerContainerAssets.HideView(uniqueId, popId);
        return popId;
    }
    void ILayerContainer.HideAllView()
    {
        while (uniqueIds.Count != 0)
        {
            int uniqueId = uniqueIds.Pop();
            ILayerContainer layerContainer = this;
            RemoveAndTryPop(uniqueId);
            layerContainer.HideViewTryPop(uniqueId);
        }
        stashDict.Clear();
    }
    private int? RemoveAndTryPop(int uniqueId)
    {
        if (uniqueIds.Count == 0
            || uniqueIds.Peek() != uniqueId)
        {
            return null;
        }
        uniqueIds.Pop();
        if (uniqueIds.Count == 0)
        {
            return null;
        }
        return uniqueIds.Pop();
    }
    
    void ILayerContainer.Stash(int uniqueId)
    {
        if (uniqueIds.Count == 0) return;
        if (!stashDict.TryGetValue(uniqueId, out Stack<int> stack))
        {
            stashDict.Add(uniqueId, stack = new Stack<int>());
        }
        foreach (int id in uniqueIds)
        {
            IView view = layerContainerAssets.GetView(id);
            view.Hide();
            stack.Push(id);
        }
        uniqueIds.Clear();
    }
    bool ILayerContainer.TryStashPop(int uniqueId, out Queue<int> popIds)
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
