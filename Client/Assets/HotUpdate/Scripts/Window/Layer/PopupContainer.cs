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
        uniqueIds.Add(uniqueId);
    }

    int? ILayerContainer.HideViewTryPop(int uniqueId)
    {
        PopAndTryPush(uniqueId);
        layerContainerAssets.HideView(uniqueId, null);
        return null;
    }
    void ILayerContainer.HideAllView()
    {
        List<int> uniqueIds = this.uniqueIds.ToList();
        foreach (var uniqueId in uniqueIds)
        {
            ILayerContainer layerContainer = this;
            PopAndTryPush(uniqueId);
            layerContainer.HideViewTryPop(uniqueId);
        }
        stashDict.Clear();
    }
    private void PopAndTryPush(int uniqueId)
    {
        uniqueIds.Remove(uniqueId);
    }
    
    void ILayerContainer.Stash(int uniqueId)
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
