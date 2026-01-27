using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class UniqueViewLoader : IViewLoader
{
    private int capacity;
    private List<Type> iteration;
    private Dictionary<Type, IView> pools;
    private Dictionary<Type, IView> actives;

    public UniqueViewLoader()
    {
        iteration = new List<Type>();
        pools = new Dictionary<Type, IView>();
        actives = new Dictionary<Type, IView>();
    }
    IViewLoader IViewLoader.SetCapacity(int capacity)
    {
        this.capacity = capacity;
        iteration = capacity > 0 ? new List<Type>(capacity) : new List<Type>();
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
        iteration.Add(type);
        pools.Add(type, view);
        if (iteration.Count > capacity)
        {
            Type removeType = iteration[0];
            iteration.RemoveAt(0);
            IView removeView = pools[removeType];
            pools.Remove(removeType);
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
            sb.AppendLine($"[{index}] => key: {pair.Key}, value: {pair.Value}");
            index++;
        }

        index = 0;
        sb.AppendLine("actives => ");
        foreach (var pair in actives)
        {
            sb.AppendLine($"[{index}] => key: {pair.Key}, value: {pair.Value}");
            index++;
        }
        return sb.ToString();
    }
}
