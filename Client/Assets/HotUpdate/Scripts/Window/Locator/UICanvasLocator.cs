using System.Collections.Generic;
using CommunityToolkit.Mvvm.DependencyInjection;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasLocator : MonoBehaviour
{
    private Transform canvasRt;
    private Canvas canvas;

    private IViewService viewService;
    private Dictionary<ViewLayer, LayerLocator> layerLocators;
    
    public void Build(IViewService viewService)
    {
        this.viewService = viewService;
        viewService.BindLocator(this);
        
        canvasRt = gameObject.AddComponent<RectTransform>();
        canvas = gameObject.AddComponent<Canvas>();
        layerLocators = new Dictionary<ViewLayer, LayerLocator>();

        DontDestroyOnLoad(gameObject);
        BindComponent();
        CreateLayerLocators();
    }

    private void BindComponent()
    {
        // Canvas 渲染模式
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        // 添加必要组件
        gameObject.AddComponent<GraphicRaycaster>();
        var scaler = gameObject.AddComponent<CanvasScaler>();
        
        // 配置CanvasScaler（适配方案）
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
    }

    private void CreateLayerLocators()
    {
        Dictionary<ViewLayer, ILayerContainer> layerContainers = viewService.GetLayerContainers();
        foreach (var pair in layerContainers)
        {
            ViewLayer viewLayer = pair.Key;
            var layerContainer = pair.Value;
            layerContainer.BindService(viewService);
            GameObject go = new GameObject(viewLayer.ToString());
            go.transform.SetParent(canvasRt);
            var layerLocator = go.AddComponent<LayerLocator>();
            layerLocator.Build(viewLayer);
            layerLocator.BindContainer(layerContainer);
            layerLocators.Add(viewLayer, layerLocator);
        }
    }

    public IViewService ViewService() => viewService;
    public LayerLocator GetLayerLocator(ViewLayer viewLayer) => layerLocators[viewLayer];
}
