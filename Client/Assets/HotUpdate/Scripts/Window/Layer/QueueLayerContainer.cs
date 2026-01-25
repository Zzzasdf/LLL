using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class QueueLayerContainer<TLayerLocator, TViewLocator, TViewLoader>: ILayerContainer
    where TLayerLocator: MonoBehaviour, ILayerLocator
    where TViewLocator: MonoBehaviour, IViewLocator
    where TViewLoader: IViewLoader, new()
{
    private readonly ViewLayer viewLayer;
    private int capacity;
    private IViewLoader viewLoader;
    
    private TLayerLocator layerLocator;

    private List<int> uniqueIds;
    private Dictionary<int, Queue<int>> stashDict;
    
    public QueueLayerContainer(ViewLayer viewLayer, int capacity)
    {
        this.viewLayer = viewLayer;
        this.capacity = capacity;
        viewLoader = new TViewLoader();
        uniqueIds = new List<int>();
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
        int? popId = PushAndTryPop(uniqueId);
        return (view, popId);
    }
    async UniTask<int?> ILayerContainer.PopViewAndTryRemove(int uniqueId)
    {
        bool result = await layerLocator.TryPopViewAsync(uniqueId);
        if (!result) return null;
        return PushAndTryPop(uniqueId);
    }
    private int? PushAndTryPop(int uniqueId)
    {
        int? popId = null;
        if (uniqueIds.Count == capacity)
        {
            popId = uniqueIds[0];
        }
        uniqueIds.Add(uniqueId);
        return popId;
    }

    int? ILayerContainer.HideViewTryPop(int uniqueId)
    {
        int? popId = RemoveAndTryPop(uniqueId);
        layerLocator.HideView(uniqueId);
        return popId;
    }
    void ILayerContainer.HideAllView()
    {
        while (uniqueIds.Count != 0)
        {
            int uniqueId = uniqueIds.First();
            ILayerContainer layerContainer = this;
            layerContainer.HideViewTryPop(uniqueId);
        }
        stashDict.Clear();
    }
    private int? RemoveAndTryPop(int uniqueId)
    {
        int index = uniqueIds.IndexOf(uniqueId);
        uniqueIds.RemoveAt(index);
        
        Type removeType = layerLocator.GetViewType(uniqueId);
        // 判断后面是否有同界面资源在启用，如有则不处理
        for (int i = index; i < uniqueIds.Count; i++)
        {
            int nextUniqueId = uniqueIds[i];
            Type nextType = layerLocator.GetViewType(nextUniqueId);
            if (removeType != nextType) continue;
            if (!layerLocator.ExistInstantiate(nextUniqueId)) continue;
            return null;
        }
        // 判断前面是否有同界面资源在启用
        for (int i = index - 1; i >= 0; i--)
        {
            int previousUniqueId = uniqueIds[i];
            Type previousType = layerLocator.GetViewType(previousUniqueId);
            if (removeType != previousType) continue;
            if (layerLocator.ExistInstantiate(previousUniqueId)) return null;
            uniqueIds.RemoveAt(i);
            return previousUniqueId;
        }
        return null;
    }
    
    void ILayerContainer.Stash(int uniqueId)
    {
        if (uniqueIds.Count == 0) return;
        if (!stashDict.TryGetValue(uniqueId, out Queue<int> queue))
        {
            stashDict.Add(uniqueId, queue = new Queue<int>());
        }
        foreach (int id in uniqueIds)
        { 
            layerLocator.PushHideView(id);
            queue.Enqueue(id);
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
