using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class MultipleViewLoader : IViewLoader
{
    private List<Type> iteration;
    private Dictionary<Type, Queue<IView>> pools;

    public MultipleViewLoader()
    {
        pools = new Dictionary<Type, Queue<IView>>();
    }
    IViewLoader IViewLoader.SetCapacity(int capacity)
    {
        iteration = new List<Type>(capacity);
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
        if (iteration.Count == iteration.Capacity)
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
        if (!pools.TryGetValue(type, out Queue<IView> queue))
        {
            pools.Add(type, queue = new Queue<IView>());
        }
        queue.Enqueue(view);
    }
}
