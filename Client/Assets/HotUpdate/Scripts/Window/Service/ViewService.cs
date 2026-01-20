using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class ViewService : IViewService
{
    private ILogService logService;
    
    private Dictionary<ViewLayer, ILayerContainer> layerContainers;
    private Dictionary<int, IView> uniqueIdCache;
    private int incrementId;

    private UICanvasLocator uiCanvasLocator;
    
    public ViewService(Dictionary<ViewLayer, ILayerContainer> layerContainers, ILogService logService)
    {
        this.logService = logService;
        
        this.layerContainers = layerContainers;
        uniqueIdCache = new Dictionary<int, IView>();
    }

    void IViewService.BindLocator(UICanvasLocator uiCanvasLocator)
    {
        this.uiCanvasLocator = uiCanvasLocator;
    }

    Dictionary<ViewLayer, ILayerContainer> IViewService.GetLayerContainers()
    {
        return layerContainers;
    }

    async UniTask<T> IViewService.ShowAsync<T>(ViewLayer viewLayer)
    {
        if (!layerContainers.TryGetValue(viewLayer, out ILayerContainer windowContainer))
        {
            logService.Error($"未支持的层级类型 {viewLayer}");
            return await new UniTask<T>(default);
        }
        
        string name = typeof(T).Name;
        var handle = YooAssets.LoadAssetAsync<GameObject>(name);
        await handle.Task;
        
        GameObject assetObject = handle.AssetObject as GameObject;
        if (assetObject == null)
        {
            logService.Error($"加载的资源不是 GameObject: {name}");
            return await new UniTask<T>(default);
        }

        if (handle.Status != EOperationStatus.Succeed)
        {

            logService.Error($"未找到该资源 {name}");
            return await new UniTask<T>(default);
        }

        GameObject instantiatedObject = Object.Instantiate(assetObject);
        if (instantiatedObject == null)
        {
            logService.Error($"实例化失败: {name}");
            return await new UniTask<T>(default);
        }
        
        int uniqueId = CreateUniqueId();
        T window = instantiatedObject.GetComponent<T>();
        window.Init(viewLayer, uniqueId);
        RectTransform windowRt = window.GameObject().GetComponent<RectTransform>();
        LayerLocator layerLocator = uiCanvasLocator.GetLayerLocator(viewLayer);
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
        logService.Debug($"{name} 创建成功！！");
        return window;
    }

    async UniTask<bool> IViewService.HideAsync(IView view)
    {
        ViewLayer viewLayer = view.GetLayer();
        if (!layerContainers.TryGetValue(viewLayer, out ILayerContainer windowContainer))
        {
            logService.Error($"未支持的层级类型 {viewLayer}");
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
            logService.Error($"无效的 {nameof(view)} UniqueId:{uniqueId}");
            return false;
        }
        uniqueIdCache.Remove(uniqueId);
        Object.DestroyImmediate(view.GameObject());
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
}
