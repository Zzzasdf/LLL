using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StackLayerContainer<TLayerLocator, TViewLocator, TViewLoader>: ILayerContainer
    where TLayerLocator: MonoBehaviour, ILayerLocator
    where TViewLocator: MonoBehaviour, IViewLocator
    where TViewLoader: IViewLoader, new()
{
    private readonly ViewLayer viewLayer;
    private IViewLoader viewLoader;

    private TLayerLocator layerLocator;

    private Stack<int> uniqueIds;
    private Dictionary<int, Stack<int>> stashDict;
    
    public StackLayerContainer(ViewLayer viewLayer, int capacity)
    {
        this.viewLayer = viewLayer;
        viewLoader = new TViewLoader().SetCapacity(capacity);
        uniqueIds = new Stack<int>();
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
        if (uniqueIds.Count > 0)
        {
            int hideId = uniqueIds.Peek();
            layerLocator.PushHideView(hideId);
        }
        uniqueIds.Push(uniqueId);
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
            int uniqueId = uniqueIds.Pop();
            ILayerContainer layerContainer = this;
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
        foreach (int id in stack)
        {
            popIds.Enqueue(id);
        }
        return true;
    }

    ViewLayer ILayerContainer.GetViewLayer() => viewLayer;
    IViewLoader ILayerContainer.GetViewLoader() => viewLoader;
    ILayerLocator ILayerContainer.GetLocator() => layerLocator;
    void ILayerContainer.AddViewLocator(GameObject goView) => goView.AddComponent<TViewLocator>();
}
