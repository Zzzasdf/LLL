using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class ViewUniqueLoader : ObjectPoolAsync<Type, IView>, IViewLoader
{
    private Dictionary<Type, IView> actives;

    public ViewUniqueLoader(int poolCapacity, int preDestroyCapacity, int preDestroyMillisecondsDelay)
        : base(poolCapacity, preDestroyCapacity, preDestroyMillisecondsDelay, OnCreate, OnDestroy)
    {
        actives = new Dictionary<Type, IView>();
    }
    private static async UniTask<IView> OnCreate(Type type)
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
    private static void OnDestroy(IView view)
    {
        UnityEngine.Object.Destroy(view.GameObject());
    }

    bool IViewLoader.TryGetActiveView(Type type, out IView view)
    {
        return actives.TryGetValue(type, out view);
    }
    bool IViewLoader.TryGetPoolView(Type type, out IView view)
    {
        if (TryGetFromPool(type, out view))
        {
            actives.Add(type, view);
            return true;
        }
        return false;
    }
    async UniTask<IView> IViewLoader.CreateView(Type type)
    {
        IView view = await Create(type);
        actives.Add(type, view);
        return view;
    }

    void IViewLoader.ReleaseView(IView view)
    {
        Type type = view.GetType();
        actives.Remove(type);
        Release(type, view);
    }

    List<int> IViewLoader.BatchAddFilter(List<Type> types, List<int> uniqueIds)
    {
        List<Type> newTypes = new List<Type>();
        List<int> newUniqueIds = new List<int>();
        for (int i = 0; i < types.Count; i++)
        {
            Type type = types[i];
            int uniqueId = uniqueIds[i];
            if (newTypes.Contains(type))
            {
                int index = newTypes.IndexOf(type);
                newTypes.RemoveAt(index);
                newUniqueIds.RemoveAt(index);
            }
            newTypes.Add(type);
            newUniqueIds.Add(uniqueId);
        }
        return newUniqueIds;
    }

    string IViewLoader.ToString()
    {
        string baseString = ToString();
        StringBuilder sb = new StringBuilder(GetType().Name);
        int index = 0;
        sb.AppendLine("actives => ");
        foreach (var pair in actives)
        {
            sb.AppendLine($"[{index}] => key: {pair.Key}, value: {pair.Value}");
            index++;
        }
        return $"{baseString}{sb}";
    }
}
