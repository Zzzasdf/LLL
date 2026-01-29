using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public partial class ViewService
{
    private Dictionary<SubViewContainerType, ISubViewContainer> subViewContainers;
    private Dictionary<Type, ISubViewConfigure> subViews;
    private Dictionary<SubViewAKA, ISubViewConfigure> subViewAKAs;

    private void InitSubViews(Dictionary<SubViewContainerType, ISubViewContainer> subViewContainers, Dictionary<SubViewContainerType, List<ISubViewConfigure>> subViews)
    {
        foreach (var pair in subViewContainers)
        {
            SubViewContainerType subViewContainerType = pair.Key;
            ISubViewContainer subViewContainer = pair.Value;
            subViewContainer.AddSubViewContainerType(subViewContainerType);
        }
        this.subViewContainers = subViewContainers;
        
        this.subViews = new Dictionary<Type, ISubViewConfigure>();
        this.subViewAKAs = new Dictionary<SubViewAKA, ISubViewConfigure>();
        foreach (var pair in subViews)
        {
            SubViewContainerType subViewContainerType = pair.Key;
            List<ISubViewConfigure> subViewConfigures = pair.Value;
            for (int i = 0; i < subViewConfigures.Count; i++)
            {
                ISubViewConfigure subViewConfigure = subViewConfigures[i];
                subViewConfigure.AddSubViewContainerType(subViewContainerType);
                
                Type subViewType = subViewConfigure.GetSubViewType();
                this.subViews.Add(subViewType, subViewConfigure);
                
                List<SubViewAKA> subViewAKAs = subViewConfigure.GetSubViewAKAs();
                for (int j = 0; j < subViewAKAs?.Count; j++)
                {
                    SubViewAKA subViewAka = subViewAKAs[j];
                    this.subViewAKAs.Add(subViewAka, subViewConfigure);
                }
            }
        }
    }
    
    private async UniTask<IView> AddSubViewAsync_Internal(ISubViewConfigure subViewConfigure)
    {
        Type type = subViewConfigure.GetSubViewType();
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
}
