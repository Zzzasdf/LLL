using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UniqueLayerContainer<TLayerLocator, TViewLocator, TViewLoader>: ILayerContainer
    where TLayerLocator: MonoBehaviour, ILayerLocator
    where TViewLocator: MonoBehaviour, IViewLocator
    where TViewLoader: IViewLoader, new()
{
    private readonly ViewLayer viewLayer;
    private IViewLoader viewLoader;

    private TLayerLocator layerLocator;
    
    private Stack<int> uniqueIds;
    private Dictionary<int, Stack<int>> stashDict;
    
    public UniqueLayerContainer(ViewLayer viewLayer, int poolCapacity)
    {
        this.viewLayer = viewLayer;
        viewLoader = new TViewLoader().SetCapacity(poolCapacity);
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
        if (uniqueIds.Count > 0)
        {
            int hideId = uniqueIds.Peek();
            layerLocator.PushHideView(hideId);
        }
        uniqueIds.Push(uniqueId);
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
            LLogger.FrameError($"请优先关闭栈顶的界面！！当前请求 uniqueId => {uniqueId}, 栈顶 uniqueId => {uniqueIds.Peek()}");
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
        while (uniqueIds.Count != 0)
        {
            int uniqueId = uniqueIds.Pop();
            layerLocator.HideView(uniqueId);
        }
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
        if (uniqueIds.Count == 0
            || uniqueIds.Peek() != uniqueId)
        {
            return false;
        }
        uniqueIds.Pop();
        if (uniqueIds.Count == 0)
        {
            return true;
        }
        pop = (uniqueIds.Peek(), -1);
        return true;
    }
    
    void ILayerContainer.Stash(int uniqueId)
    {
        if (uniqueIds.Count == 0) return;
        if (!stashDict.TryGetValue(uniqueId, out Stack<int> stack))
        {
            stashDict.Add(uniqueId, stack = new Stack<int>());
        }
        while (uniqueIds.Count > 0)
        {
            int id = uniqueIds.Pop();
            stack.Push(id);
            layerLocator.PushHideView(id);
        }
    }
    bool ILayerContainer.TryStashPop(int uniqueId, out Queue<int> popIds)
    {
        popIds = null;
        if (!stashDict.Remove(uniqueId, out Stack<int> stack))
        {
            return false;
        }
        popIds = new Queue<int>();
        while (stack.Count > 0)
        {
            int id = stack.Pop();
            popIds.Enqueue(id);
        }
        foreach (var id in uniqueIds)
        {
            stack.Push(id);
        }
        while (stack.Count > 0)
        {
            int id = stack.Pop();
            popIds.Enqueue(id);
        }
        uniqueIds.Clear();
        foreach (var id in popIds)
        {
            uniqueIds.Push(id);
        }
        int popId = uniqueIds.Peek();
        popIds.Clear();
        popIds.Enqueue(popId);
        return popIds.Count > 0;
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
