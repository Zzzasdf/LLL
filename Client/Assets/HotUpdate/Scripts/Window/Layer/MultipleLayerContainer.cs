using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MultipleLayerContainer<TLayerLocator, TViewLocator, TViewLoader>: ILayerContainer
    where TLayerLocator: MonoBehaviour, ILayerLocator
    where TViewLocator: MonoBehaviour, IViewLocator
    where TViewLoader: IViewLoader, new()
{
    private readonly ViewLayer viewLayer;
    private IViewLoader viewLoader;
    
    private TLayerLocator layerLocator;

    private List<int> uniqueIds;
    private Dictionary<int, Stack<int>> stashDict;

    public MultipleLayerContainer(ViewLayer viewLayer, int poolCapacity)
    {
        this.viewLayer = viewLayer;
        viewLoader = new TViewLoader().SetCapacity(poolCapacity);
        uniqueIds = new List<int>();
        stashDict = new Dictionary<int, Stack<int>>();
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
        uniqueIds.Add(uniqueId);
        return (view, null);
    }
    async UniTask<int?> ILayerContainer.PopViewAndTryRemove(int uniqueId, int siblingIndex)
    {
        await layerLocator.TryPopViewAsync(uniqueId, siblingIndex);
        return null;
    }
    async UniTask<int?> ILayerContainer.PopViewAndTryRemove(Queue<int> uniqueIds)
    {
        await layerLocator.TryPopViewAsync(uniqueIds);
        return null;
    }
    
    (int? popId, int siblingIndex) ILayerContainer.HideViewTryPop(int uniqueId)
    {
        if (!TryRemoveAndTryPop(uniqueId, out (int? popId, int siblingIndex) pop))
        {
            LLogger.FrameError($"当前关闭的界面不存在！！当前请求 uniqueId => {uniqueId}");
        }
        else
        {
            layerLocator.HideView(uniqueId);
        }
        return pop;
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
            Stack<int> stack = pair.Value;
            foreach (var uniqueId in stack)
            {
                layerLocator.HideView(uniqueId);
            }
        }
        stashDict.Clear();
    }

    private bool TryRemoveAndTryPop(int uniqueId, out (int? popId, int siblingIndex) pop)
    {
        pop = (null, -1);
        int index = uniqueIds.IndexOf(uniqueId);
        if (index < 0) return false;
        uniqueIds.RemoveAt(index);
        Type removeType = layerLocator.GetViewType(uniqueId);
        int? nextUniqueId = null;
        for (int i = uniqueIds.Count - 1; i >= 0; i--)
        {
            int previousUniqueId = uniqueIds[i];
            Type previousType = layerLocator.GetViewType(previousUniqueId);
            if (previousType != removeType)
            {
                if (layerLocator.ExistInstantiate(previousUniqueId))
                {
                    nextUniqueId = previousUniqueId;
                }
            }
            else
            {
                if (!layerLocator.ExistInstantiate(previousUniqueId))
                {
                    if (!nextUniqueId.HasValue)
                    {
                        pop = (previousUniqueId, -1);
                        return true;
                    }
                    layerLocator.GetView(uniqueId).GameObject().transform.SetAsLastSibling();
                    int siblingIndex = layerLocator.GetView(nextUniqueId.Value).GameObject().transform.GetSiblingIndex();
                    pop = (previousUniqueId, siblingIndex);
                }
                return true;
            }
        }
        return true;
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
            layerLocator.PushHideView(id);
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
        for (int i = uniqueIds.Count - 1; i >= 0; i--)
        {
            int id = uniqueIds[i];
            popIds.Enqueue(id);
        }
        foreach (var id in stack)
        {
            popIds.Enqueue(id);
            uniqueIds.Insert(0, id);
        }
        return true;
    }

    ViewLayer ILayerContainer.GetViewLayer() => viewLayer;
    IViewLoader ILayerContainer.GetViewLoader() => viewLoader;
    ILayerLocator ILayerContainer.GetLocator() => layerLocator;
    void ILayerContainer.AddViewLocator(GameObject goView) => goView.AddComponent<TViewLocator>();

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
