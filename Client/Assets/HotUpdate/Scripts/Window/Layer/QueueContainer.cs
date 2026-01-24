using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class QueueContainer : ILayerContainer
{
    private readonly LayerContainerAssets layerContainerAssets;
    private readonly int capacity;
    
    private List<int> uniqueIds;
    private Dictionary<int, Queue<int>> stashDict;

    public QueueContainer(ViewLayer viewLayer, int capacity)
    {
        layerContainerAssets = new LayerContainerAssets(viewLayer, isMultiple: false);
        this.capacity = capacity;
        uniqueIds = new List<int>();
        stashDict = new Dictionary<int, Queue<int>>();
    }
    
    void ILayerContainer.BindLocator(ILayerLocator layerLocator) => layerContainerAssets.BindLocator(layerLocator);
    
    async UniTask<(IView view, int? removeId)> ILayerContainer.ShowViewAndTryRemoveAsync(Type type)
    {
        IView view = await layerContainerAssets.ShowViewAsync(type);
        int uniqueId = view.GetUniqueId();
        int? popId = PushAndTryPop(uniqueId);
        return (view, popId);
    }
    int? ILayerContainer.PopViewAndTryRemove(int uniqueId)
    {
        if (!layerContainerAssets.TryPopView(uniqueId))
        {
            return null;
        }
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
        int? popId = PopAndTryPush(uniqueId);
        layerContainerAssets.HideView(uniqueId, popId);
        return null;
    }
    void ILayerContainer.HideAllView()
    {
        while (uniqueIds.Count != 0)
        {
            int uniqueId = uniqueIds.First();
            ILayerContainer layerContainer = this;
            PopAndTryPush(uniqueId);
            layerContainer.HideViewTryPop(uniqueId);
        }
        stashDict.Clear();
    }
    private int? PopAndTryPush(int uniqueId)
    {
        int index = uniqueIds.IndexOf(uniqueId);
        uniqueIds.RemoveAt(index);
        IView removeView = layerContainerAssets.GetView(uniqueId);
        
        // 判断后面是否有同界面资源在启用，如有则不处理
        for (int i = index; i < uniqueIds.Count; i++)
        {
            int nextUniqueId = uniqueIds[i];
            IView nextView = layerContainerAssets.GetView(nextUniqueId);
            if (removeView != nextView) continue;
            return null;
        }
        // 判断前面是否有同界面资源在启用，如有则设置到对应位置
        Transform removeViewTra = removeView.GameObject().transform;
        int removeSiblingIndex = removeViewTra.GetSiblingIndex();
        HashSet<IView> hashSet = new HashSet<IView>();
        for (int i = index - 1; i >= 0; i--)
        {
            int previousUniqueId = uniqueIds[i];
            IView previousView = layerContainerAssets.GetView(previousUniqueId);
            if (removeView != previousView)
            {
                hashSet.Add(previousView);
            }
            else
            {
                removeViewTra.SetSiblingIndex(removeSiblingIndex - hashSet.Count);
                return previousUniqueId;
            }
        }
        if (removeView.GetRefCount() - 1 > 0)
        {
            // 在贮藏有存在
            removeView.Hide();
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
