using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

public class WindowService : IWindowService
{
    private ILogService logService;

    private Canvas canvas;
    private Transform canvasTra;
    
    private Dictionary<WindowLayer, IWindowContainer> containers;
    private Dictionary<int, IWindow> uniqueIdCache;
    private int incrementId;
    
    public WindowService(ILogService logService)
    {
        this.logService = logService;

        // 创建Canvas
        canvas = CreateBasicCanvas("UICanvas");
        canvasTra = canvas.transform;
        containers = new Dictionary<WindowLayer, IWindowContainer>();
        uniqueIdCache = new Dictionary<int, IWindow>();
        
        containers.Add(WindowLayer.Permanent, CreateUILayer<QueueContainer>(WindowLayer.Permanent)
            .Init(this, WindowLayer.Permanent, -1, logService));
        containers.Add(WindowLayer.Windows, CreateUILayer<StackContainer>(WindowLayer.Windows)
            .Init(this, WindowLayer.Windows, 8, logService));
        containers.Add(WindowLayer.Popups, CreateUILayer<PopupContainer>(WindowLayer.Popups)
            .Init(this, WindowLayer.Popups, 8, logService));
        containers.Add(WindowLayer.Tips, CreateUILayer<PopupContainer>(WindowLayer.Tips)
            .Init(this, WindowLayer.Tips, 8, logService));
        containers.Add(WindowLayer.System, CreateUILayer<QueueContainer>(WindowLayer.System)
            .Init(this, WindowLayer.System, 1, logService));
    }

    async UniTask<T> IWindowService.ShowAsync<T>(WindowLayer windowLayer)
    {
        if (!containers.TryGetValue(windowLayer, out IWindowContainer windowContainer))
        {
            logService.Error($"未支持的层级类型 {windowLayer}");
            return await new UniTask<T>(default);
        }
        int uniqueId = CreateUniqueId();
        
        T window = null;
        string name = nameof(T);
        var handle = YooAssets.LoadAssetAsync<GameObject>(name);
        await handle.Task;
        if (handle.Status == EOperationStatus.Succeed)
        {
            window = Object.Instantiate(handle.AssetObject) as T;
            window.Init(windowLayer, uniqueId);
            RectTransform windowTra = window.GameObject().GetComponent<RectTransform>();
            windowTra.SetParent(windowContainer.GetTransform());
            windowTra.localPosition = Vector3.zero;
            windowTra.localScale = Vector3.one;
            windowTra.anchoredPosition = Vector2.zero;
            
            uniqueIdCache.Add(uniqueId, window);
            await windowContainer.AddAsync(window);
        }
        else
        {
            logService.Error($"未找到该资源 {name}");
        }
        return window;
    }

    async UniTask<bool> IWindowService.HideAsync(IWindow window)
    {
        WindowLayer windowLayer = window.GetLayer();
        if (!containers.TryGetValue(windowLayer, out IWindowContainer windowContainer))
        {
            logService.Error($"未支持的层级类型 {windowLayer}");
            return false;
        }
        bool result = await windowContainer.RemoveAsync(window);
        if (!result)
        {
            return false;
        }
        int uniqueId = window.GetUniqueId();
        if (!uniqueIdCache.ContainsKey(uniqueId))
        {
            logService.Error($"无效的 {nameof(window)} UniqueId:{uniqueId}");
            return false;
        }
        uniqueIdCache.Remove(uniqueId);
        Object.DestroyImmediate(window.GameObject());
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
    
    // 创建基础 Canvas
    private Canvas CreateBasicCanvas(string canvasName)
    {
        // 创建Canvas GameObject
        GameObject canvasGO = new GameObject(canvasName);
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        
        // 设置Canvas渲染模式
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        // 添加必要组件
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        GraphicRaycaster raycaster = canvasGO.AddComponent<GraphicRaycaster>();
        
        // 配置CanvasScaler（适配方案）
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        return canvas;
    }
    
    // 创建UI层级容器
    private T CreateUILayer<T>(WindowLayer windowLayer) where T: MonoBehaviour, IWindowContainer
    {
        GameObject layerGO = new GameObject(windowLayer.ToString());
        layerGO.transform.SetParent(canvasTra);
        
        RectTransform rt = layerGO.AddComponent<RectTransform>();
        
        // 设置全屏拉伸
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        
        // 添加CanvasGroup用于控制整组UI
        CanvasGroup canvasGroup = layerGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        // 添加容器组件
        T container = layerGO.AddComponent<T>();
        
        return container;
    }
}
