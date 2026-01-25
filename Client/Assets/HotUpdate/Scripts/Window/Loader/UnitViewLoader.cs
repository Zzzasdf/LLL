using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class UnitViewLoader : IViewLoader
{
    IViewLoader IViewLoader.SetCapacity(int capacity) => this;

    bool IViewLoader.TryGetActiveView(Type type, out IView view)
    {
        view = default;
        return false;
    }

    bool IViewLoader.TryGetPoolView(Type type, out IView view)
    {
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
        UnityEngine.Object.Destroy(view.GameObject());
    }
}
