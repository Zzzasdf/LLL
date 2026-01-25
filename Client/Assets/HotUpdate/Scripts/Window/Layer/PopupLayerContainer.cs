using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PopupLayerContainer<TLayerLocator, TViewLocator, TViewLoader>: ILayerContainer
    where TLayerLocator: MonoBehaviour, ILayerLocator
    where TViewLocator: MonoBehaviour, IViewLocator
    where TViewLoader: IViewLoader, new()
{
    private readonly ViewLayer viewLayer;
    private readonly int warmCapacity;
    private IViewLoader viewLoader;
    
    private TLayerLocator layerLocator;

    private HashSet<int> uniqueIds;
    private Dictionary<int, Queue<int>> stashDict;

    public PopupLayerContainer(ViewLayer viewLayer, int warmCapacity)
    {
        this.viewLayer = viewLayer;
        this.warmCapacity = warmCapacity;
        uniqueIds = new HashSet<int>();
        stashDict = new Dictionary<int, Queue<int>>();
    }
    
    ILayerLocator ILayerContainer.AddLocator(GameObject goLocator)
    {
        layerLocator = goLocator.AddComponent<TLayerLocator>();
        return layerLocator;
    }
    
    async UniTask<(IView view, int? removeId)> ILayerContainer.ShowViewAndTryRemoveAsync(Type type)
    {
        IView view = await layerLocator.ShowViewAsync(type);
        int uniqueId = view.GetUniqueId();
        PushAndTryPop(uniqueId);
        return (view, null);
    }
    async UniTask<int?> ILayerContainer.PopViewAndTryRemove(int uniqueId)
    {
        bool result = await layerLocator.TryPopViewAsync(uniqueId);
        if (!result) return null;
        PushAndTryPop(uniqueId);
        return null;
    }
    private void PushAndTryPop(int uniqueId)
    {
        if (uniqueIds.Count == warmCapacity)
        {
            LLogger.FrameWarning($"{nameof(StackLayerContainer<TLayerLocator, TViewLocator, TViewLoader>)} 的容量已超过预警值：{warmCapacity}");
        }
        uniqueIds.Add(uniqueId);
    }

    int? ILayerContainer.HideViewTryPop(int uniqueId)
    {
        RemoveAndTryPop(uniqueId);
        layerLocator.HideView(uniqueId);
        return null;
    }
    void ILayerContainer.HideAllView()
    {
        List<int> uniqueIds = this.uniqueIds.ToList();
        foreach (var uniqueId in uniqueIds)
        {
            ILayerContainer layerContainer = this;
            layerContainer.HideViewTryPop(uniqueId);
        }
        stashDict.Clear();
    }
    private void RemoveAndTryPop(int uniqueId)
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
            layerLocator.PushHideView(id);
            stack.Enqueue(id);
        }
        uniqueIds.Clear();
    }
    bool ILayerContainer.TryStashPop(int uniqueId, out Queue<int> popIds)
    {
        return stashDict.Remove(uniqueId, out popIds);
    }

    ViewLayer ILayerContainer.GetViewLayer() => viewLayer;
    IViewLoader ILayerContainer.GetViewLoader() => viewLoader;
    ILayerLocator ILayerContainer.GetLocator() => layerLocator;
    void ILayerContainer.AddViewLocator(GameObject goView) => goView.AddComponent<TViewLocator>();
}
