using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class SingleViewLoader : IViewLoader
{
    private List<Type> iteration;
    private Dictionary<Type, IView> pools;
    private Dictionary<Type, IView> actives;

    public SingleViewLoader()
    {
        iteration = new List<Type>();
        pools = new Dictionary<Type, IView>();
        actives = new Dictionary<Type, IView>();
    }
    IViewLoader IViewLoader.SetCapacity(int capacity)
    {
        iteration = new List<Type>(capacity);
        return this;
    }

    bool IViewLoader.TryGetActiveView(Type type, out IView view)
    {
        return actives.TryGetValue(type, out view);
    }

    bool IViewLoader.TryGetPoolView(Type type, out IView view)
    {
        if (pools.Remove(type, out view))
        {
            iteration.Remove(type);
            actives.Add(type, view);
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
        actives.Add(type, view);
        return view;
    }

    void IViewLoader.ReleaseView(IView view)
    {
        Type type = view.GetType();
        actives.Remove(type);
        if (iteration.Count == iteration.Capacity)
        {
            Type removeType = iteration[0];
            iteration.RemoveAt(0);
            IView removeView = pools[removeType];
            pools.Remove(removeType);
            UnityEngine.Object.Destroy(removeView.GameObject());
        }
        iteration.Add(type);
        pools.Add(type, view);
    }
}
