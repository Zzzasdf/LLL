using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class SingleViewLoader : IViewLoader
{
    private Dictionary<Type, IView> pools;
    private Dictionary<Type, IView> actives;

    public SingleViewLoader()
    {
        pools = new Dictionary<Type, IView>();
        actives = new Dictionary<Type, IView>();
    }

    bool IViewLoader.TryGetActiveView(Type type, out IView view)
    {
        return actives.TryGetValue(type, out view);
    }

    bool IViewLoader.TryGetPoolView(Type type, out IView view)
    {
        if (pools.Remove(type, out view))
        {
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
        pools.Add(type, view);
    }
}
