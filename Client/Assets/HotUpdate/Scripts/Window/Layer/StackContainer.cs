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

    UniTask<IView> ILayerContainer.ShowViewAsync(Type type) => layerContainerAssets.ShowViewAsync(type);
    void ILayerContainer.HideView(int uniqueId) => layerContainerAssets.HideView(uniqueId);
    bool ILayerContainer.TryPopView(int uniqueId) => layerContainerAssets.TryPopView(uniqueId);

    void ILayerContainer.HideAllView()
    {
        while (uniqueIds.Count != 0)
        {
            int uniqueId = uniqueIds.Pop();
            ILayerContainer layerContainer = this;
            layerContainer.PopAndTryPush(uniqueId, out _);
            layerContainer.HideView(uniqueId);
        }
        stashDict.Clear();
    }

    bool ILayerContainer.PushAndTryPop(int uniqueId, out int popId)
    {
        popId = 0;
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
        return false;
    }
    bool ILayerContainer.PopAndTryPush(int uniqueId, out int pushId)
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
    
    void ILayerContainer.StashPush(int uniqueId)
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
