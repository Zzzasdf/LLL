using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

public class PopupContainer: ILayerContainer
{
    private readonly LayerContainerAssets layerContainerAssets;
    private readonly int warmCapacity;
    
    private HashSet<int> uniqueIds;
    private Dictionary<int, Queue<int>> stashDict;

    public PopupContainer(ViewLayer viewLayer, int warmCapacity)
    {
        layerContainerAssets = new LayerContainerAssets(viewLayer, isMultiple: true);
        this.warmCapacity = warmCapacity;
        uniqueIds = new HashSet<int>();
        stashDict = new Dictionary<int, Queue<int>>();
    }

    void ILayerContainer.BindLocator(ILayerLocator layerLocator) => layerContainerAssets.BindLocator(layerLocator);

    UniTask<IView> ILayerContainer.ShowViewAsync(Type type) => layerContainerAssets.ShowViewAsync(type);
    void ILayerContainer.HideView(int uniqueId) => layerContainerAssets.HideView(uniqueId);
    bool ILayerContainer.TryPopView(int uniqueId) => layerContainerAssets.TryPopView(uniqueId);

    void ILayerContainer.HideAllView()
    {
        List<int> uniqueIds = this.uniqueIds.ToList();
        foreach (var uniqueId in uniqueIds)
        {
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
        uniqueIds.Add(uniqueId);
        return false;
    }
    bool ILayerContainer.PopAndTryPush(int uniqueId, out int pushId)
    {
        pushId = 0;
        uniqueIds.Remove(uniqueId);
        return false;
    }
    
    void ILayerContainer.StashPush(int uniqueId)
    {
        if (uniqueIds.Count == 0) return;
        if (!stashDict.TryGetValue(uniqueId, out Queue<int> stack))
        {
            stashDict.Add(uniqueId, stack = new Queue<int>());
        }
        foreach (int id in uniqueIds)
        {
            IView view = layerContainerAssets.GetView(id);
            view.Hide();
            stack.Enqueue(id);
        }
        uniqueIds.Clear();
    }
    bool ILayerContainer.TryStashPop(int uniqueId, out Queue<int> popIds)
    {
        return stashDict.Remove(uniqueId, out popIds);
    }
}
