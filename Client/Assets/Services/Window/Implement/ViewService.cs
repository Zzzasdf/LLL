using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class ViewService : ObservableRecipient,
    IViewService,
    IRecipient<ViewShowAsyncRequestEvent>,
    IRecipient<ViewHideAsyncRequestEvent>,
    IRecipient<ViewAllHideAsyncRequestEvent>
{
    private Dictionary<ViewLayer, ILayerContainer> layerContainers;
    private Dictionary<Type, ViewLayer> viewLayers;
    private Dictionary<int, IView> uniqueIdCache;
    private int incrementId;

    private IUICanvasLocator uiCanvasLocator;
    
    public ViewService(Dictionary<ViewLayer, ILayerContainer> layerContainers, Dictionary<ViewLayer, List<Type>> viewLayers)
    {
        this.layerContainers = layerContainers;
        this.viewLayers = new Dictionary<Type, ViewLayer>();
        foreach (var pair  in viewLayers)
        {
            ViewLayer viewLayer = pair.Key;
            List<Type> types = pair.Value;
            for (int i = 0; i < types.Count; i++)
            {
                Type type = types[i];
                this.viewLayers.Add(type, viewLayer);
            }
        }
        uniqueIdCache = new Dictionary<int, IView>();
        IsActive = true;
    }

    void IViewService.BindLocator(IUICanvasLocator uiCanvasLocator)
    {
        this.uiCanvasLocator = uiCanvasLocator;
    }

    Dictionary<ViewLayer, ILayerContainer> IViewService.GetLayerContainers()
    {
        return layerContainers;
    }

    private async UniTask<IView> ShowAsync_Internal(Type type)
    {
        if (!viewLayers.TryGetValue(type, out ViewLayer viewLayer))
        {
            LLogger.FrameError($"{type} 未指定 {typeof(ViewLayer)}");
            return await new UniTask<IView>(default);
        }
        if (!layerContainers.TryGetValue(viewLayer, out ILayerContainer windowContainer))
        {
            LLogger.FrameError($"未支持的层级类型 {viewLayer}");
            return await new UniTask<IView>(default);
        }
        
        string name = type.Name;
        var handle = YooAssets.LoadAssetAsync<GameObject>(name);
        await handle.Task;
        
        if (handle.Status != EOperationStatus.Succeed)
        {
            LLogger.FrameError($"未找到该资源 {name}");
            return await new UniTask<IView>(default);
        }
        
        GameObject assetObject = handle.AssetObject as GameObject;
        if (assetObject == null)
        {
            LLogger.FrameError($"加载的资源不是 GameObject: {name}");
            return await new UniTask<IView>(default);
        }

        GameObject instantiatedObject = UnityEngine.Object.Instantiate(assetObject);
        if (instantiatedObject == null)
        {
            LLogger.FrameError($"实例化失败: {name}");
            return await new UniTask<IView>(default);
        }
        
        int uniqueId = CreateUniqueId();
        IView window = instantiatedObject.GetComponent<IView>();
        window.Init(viewLayer, uniqueId);
        GameObject windowGo = window.GameObject();
        RectTransform windowRt = windowGo.GetComponent<RectTransform>();
        ILayerLocator layerLocator = uiCanvasLocator.GetLayerLocator(viewLayer);
        windowRt.SetParent(layerLocator.GetRectTransform());
        windowRt.localPosition = Vector3.zero;
        windowRt.localScale = Vector3.one;
        windowRt.anchoredPosition = Vector2.zero;
        windowRt.anchorMin = Vector2.zero;
        windowRt.anchorMax = Vector2.one;
        windowRt.offsetMin = Vector2.zero;
        windowRt.offsetMax = Vector2.zero;
            
        uniqueIdCache.Add(uniqueId, window);
        await windowContainer.AddAsync(window);
        LLogger.FrameLog($"{name} 创建成功！！");
        return window;
    }

    private async UniTask<bool> HideAllAsync_Internal()
    {
        foreach (var pair in layerContainers)
        {
            ILayerContainer layerContainer = pair.Value;
            if (await layerContainer.TryPop(out IView view))
            {
                await HideAsync_Internal(view);
            }
        }
        return await UniTask.FromResult(true);
    }

    private async UniTask<bool> HideAsync_Internal(IView view)
    {
        ViewLayer viewLayer = view.GetLayer();
        if (!layerContainers.TryGetValue(viewLayer, out ILayerContainer windowContainer))
        {
            LLogger.FrameError($"未支持的层级类型 {viewLayer}");
            return false;
        }
        bool result = await windowContainer.RemoveAsync(view);
        if (!result)
        {
            return false;
        }
        int uniqueId = view.GetUniqueId();
        if (!uniqueIdCache.ContainsKey(uniqueId))
        {
            LLogger.FrameError($"无效的 {nameof(view)} UniqueId:{uniqueId}");
            return false;
        }
        uniqueIdCache.Remove(uniqueId);
        UnityEngine.Object.Destroy(view.GameObject());
        return true;
    }
    
    /// 生成唯一 id
    private int CreateUniqueId()
    {
        do
        {
            incrementId++;
        } while (uniqueIdCache.ContainsKey(incrementId));
        return incrementId;
    }

    void IRecipient<ViewShowAsyncRequestEvent>.Receive(ViewShowAsyncRequestEvent message)
    {
        message.Reply(ShowAsync_Internal(message.Type).AsTask());
    }

    void IRecipient<ViewHideAsyncRequestEvent>.Receive(ViewHideAsyncRequestEvent message)
    {
        message.Reply(HideAsync_Internal(message.View).AsTask());
    }

    void IRecipient<ViewAllHideAsyncRequestEvent>.Receive(ViewAllHideAsyncRequestEvent message)
    {
        message.Reply(HideAllAsync_Internal().AsTask());
    }
}
