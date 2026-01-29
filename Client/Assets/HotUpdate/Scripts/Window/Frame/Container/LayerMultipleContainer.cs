using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LayerMultipleContainer<TLayerLocator, TViewLocator, TViewLoader>: ILayerContainer
    where TLayerLocator: MonoBehaviour, ILayerLocator
    where TViewLocator: MonoBehaviour, IViewLocator
    where TViewLoader: IViewLoader
{
    private ViewLayer viewLayer;
    private IViewLoader viewLoader;
    
    private TLayerLocator layerLocator;

    private List<int> uniqueIds;
    private Dictionary<int, List<int>> stashDict;

    public LayerMultipleContainer(int poolCapacity, int preDestroyCapacity = 10, int preDestroyMillisecondsDelay = 1000)
    {
        viewLoader = (TViewLoader)Activator.CreateInstance
        (
            typeof(TViewLoader), poolCapacity, preDestroyCapacity, preDestroyMillisecondsDelay
        );
        uniqueIds = new List<int>();
        stashDict = new Dictionary<int, List<int>>();
    }

    void ILayerContainer.AddLayer(ViewLayer viewLayer) => this.viewLayer = viewLayer;

    ILayerLocator ILayerContainer.AddLocator(GameObject goLocator)
    {
        layerLocator = goLocator.AddComponent<TLayerLocator>();
        return layerLocator;
    }
    
    async UniTask<(IView view, int? removeId)> ILayerContainer.ShowViewAndTryRemoveAsync(Type type)
    {
        IView view = await layerLocator.ShowViewAsync(type);
        IViewLocator viewLocator = view.GetLocator();
        int uniqueId = viewLocator.GetUniqueId();
        uniqueIds.Add(uniqueId);
        return (view, null);
    }
    async UniTask<List<int>> ILayerContainer.PopViewAndTryRemove(List<int> popIds)
    {
        await layerLocator.TryPopViewAsync(popIds);
        return null;
    }
    
    List<int> ILayerContainer.HideViewTryPop(int uniqueId)
    {
        if (!TryRemoveAndTryPop(uniqueId, out List<int> popIds))
        {
            LLogger.FrameError($"当前关闭的界面不存在！！当前请求 uniqueId => {uniqueId}");
        }
        else
        {
            layerLocator.HideView(uniqueId);
        }
        return popIds;
    }
    void ILayerContainer.HideAllView()
    {
        ILayerContainer layerContainer = this;
        layerContainer.HideAllActivateView();
        layerContainer.HideAllStashView();
    }
    void ILayerContainer.HideAllActivateView()
    {
        for (int i = uniqueIds.Count - 1; i >= 0; i--)
        {
            int uniqueId = uniqueIds[i];
            layerLocator.HideView(uniqueId);
        }
        uniqueIds.Clear();
    }
    void ILayerContainer.HideAllStashView()
    {
        foreach (var pair in stashDict)
        {
            List<int> list = pair.Value;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                int uniqueId = uniqueIds[i];
                layerLocator.HideView(uniqueId);
            }
        }
        stashDict.Clear();
    }

    private bool TryRemoveAndTryPop(int uniqueId, out List<int> popIds)
    {
        popIds = null;
        uniqueIds.Remove(uniqueId);
        if (uniqueIds.Count == 0) return true;
        popIds = new List<int>();
        for (int i = 0; i < uniqueIds.Count; i++)
        {
            popIds.Add(uniqueIds[i]);
        }
        return true;
    }
    
    void ILayerContainer.Stash(int uniqueId)
    {
        if (uniqueIds.Count == 0) return;
        if (!stashDict.TryGetValue(uniqueId, out List<int> list))
        {
            stashDict.Add(uniqueId, list = new List<int>());
        }
        foreach (int id in uniqueIds)
        {
            layerLocator.PushHideView(id);
            list.Add(id);
        }
        uniqueIds.Clear();
    }

    bool ILayerContainer.TryStashPop(int uniqueId, out List<int> popIds)
    {
        if (!stashDict.Remove(uniqueId, out popIds))
        {
            return false;
        }
        for (int i = 0; i < popIds.Count; i++)
        {
            int id = popIds[i];
            uniqueIds.Insert(i, id);
        }
        popIds.Clear();
        for (int i = 0; i < uniqueIds.Count; i++)
        {
            int id = uniqueIds[i];
            popIds.Add(id);
        }
        return true;
    }

    void ILayerContainer.StashClear(int uniqueId)
    {
        stashDict.Remove(uniqueId);
    }

    ViewLayer ILayerContainer.GetViewLayer() => viewLayer;
    IViewLoader ILayerContainer.GetViewLoader() => viewLoader;
    ILayerLocator ILayerContainer.GetLocator() => layerLocator;
    IViewLocator ILayerContainer.AddViewLocator(GameObject goView) => goView.AddComponent<TViewLocator>();

    string ILayerContainer.ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"uniqueIds Count => {uniqueIds.Count}");
        int index = 0;
        foreach (var uniqueId in uniqueIds)
        {
            sb.AppendLine($"[{index}] => {uniqueId}");
            index++;
        }

        sb.AppendLine($"stashDict Count => {stashDict.Count}");
        index = 0;
        foreach (var pair in stashDict)
        {
            int queueIndex = 0;
            StringBuilder sbQueue = new StringBuilder();
            foreach (var uniqueId in pair.Value)
            {
                sbQueue.Append($"[{queueIndex}] => {uniqueId}");
            }
            sb.AppendLine($"[{index}] => key: {pair.Key}, value: {sbQueue}");
            index++;
        }
        return sb.ToString();
    }
}
