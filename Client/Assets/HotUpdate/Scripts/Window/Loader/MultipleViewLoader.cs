using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class MultipleViewLoader : IViewLoader
{
    private int capacity;
    private List<Type> iteration;
    private Dictionary<Type, Queue<IView>> pools;

    public MultipleViewLoader()
    {
        pools = new Dictionary<Type, Queue<IView>>();
    }
    IViewLoader IViewLoader.SetCapacity(int capacity)
    {
        this.capacity = capacity;
        iteration = capacity > 0 ? new List<Type>(capacity) : new List<Type>();
        return this;
    }
    
    bool IViewLoader.TryGetActiveView(Type type, out IView view)
    {
        view = default;
        return false;
    }

    bool IViewLoader.TryGetPoolView(Type type, out IView view)
    {
        if (pools.TryGetValue(type, out Queue<IView> queue)
            && queue.Count > 0)
        {
            iteration.Remove(type);
            view = queue.Dequeue();
            return true;
        }
        view = default;
        return false;
    }

    async UniTask<IView> IViewLoader.CreateView(Type type)
    {
        string name = type.Name;
        var handle = YooAssets.LoadAssetAsync<GameObject>(name);
        await handle.Task;
        if (handle.Status != EOperationStatus.Succeed)
        {
            LLogger.FrameError($"未找到该资源 {name}");
            return default;
        }
        GameObject assetObject = handle.AssetObject as GameObject;
        if (assetObject == null)
        {
            LLogger.FrameError($"加载的资源不是 GameObject: {name}");
            return default;
        }
        GameObject instantiatedObject = UnityEngine.Object.Instantiate(assetObject);
        if (instantiatedObject == null)
        {
            LLogger.FrameError($"实例化失败: {name}");
            return default;
        }
        IView view = instantiatedObject.GetComponent(type) as IView;
        return view;
    }

    void IViewLoader.ReleaseView(IView view)
    {
        Type type = view.GetType();
        iteration.Add(type);
        if (!pools.TryGetValue(type, out Queue<IView> queue))
        {
            pools.Add(type, queue = new Queue<IView>());
        }
        queue.Enqueue(view);
        if (iteration.Count > capacity)
        {
            Type removeType = iteration[0];
            iteration.RemoveAt(0);
            Queue<IView> removeQueue = pools[removeType];
            IView removeView = removeQueue.Dequeue();
            if (removeQueue.Count == 0)
            {
                pools.Remove(removeType);
            }
            UnityEngine.Object.Destroy(removeView.GameObject());
        }
    }
    
    string IViewLoader.ToString()
    {
        StringBuilder sb = new StringBuilder(GetType().Name);
        sb.AppendLine($" capacity => {capacity}");

        int index = 0;
        sb.AppendLine($"iteration => ");
        foreach (var item in iteration)
        {
            sb.AppendLine($"[{index}] => {item}");
            index++;
        }
        
        index = 0;
        sb.AppendLine("pools => ");
        foreach (var pair in pools)
        {
            int queueIndex = 0;
            StringBuilder sbQueue = new StringBuilder();
            foreach (var uniqueId in pair.Value)
            {
                sbQueue.Append($"[{queueIndex}] => {uniqueId}");
                queueIndex++;
            }
            sb.AppendLine($"[{index}] => key: {pair.Key}, value: {sbQueue}");
            index++;
        }
        return sb.ToString();
    }
}
