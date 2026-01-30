using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasLocator : MonoBehaviour, IUICanvasLocator
{
    private IViewService viewService;
    private Dictionary<ViewLayer, ILayerLocator> layerLocators;
    private Dictionary<SubViewCollect, ISubViewCollectLocator> subViewDisplayLocators;

    private GameObject go;
    private Transform rt;
    private Canvas canvas;
    private GraphicRaycaster graphicRaycaster;
    private CanvasScaler canvasScaler;
    
    [SerializeField]
    private ViewModelGenerator viewModelGenerator = ViewModelGenerator.Default;
    
    public void Build(IViewService viewService)
    {
        this.viewService = viewService;
        layerLocators = new Dictionary<ViewLayer, ILayerLocator>();
        subViewDisplayLocators = new Dictionary<SubViewCollect, ISubViewCollectLocator>();
        
        go = gameObject;
        rt = go.AddComponent<RectTransform>();
        
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        graphicRaycaster = go.AddComponent<GraphicRaycaster>();
        
        canvasScaler = go.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.matchWidthOrHeight = 0.5f;
        
        DontDestroyOnLoad(go);
        
        CreateLayerLocators();
        CreateSubViewCollectLocators();
    }

    private void CreateLayerLocators()
    {
        ILayerConfigures layerConfigures = viewService.GetLayerConfigures();
        foreach (var pair in layerConfigures)
        {
            ViewLayer viewLayer = pair.Key;
            ILayerConfigure layerConfigure = pair.Value;
            GameObject goLocator = new GameObject(viewLayer.ToString());
            goLocator.transform.SetParent(rt);

            ILayerLocator layerLocator = layerConfigure.AddLayerLocator(goLocator);
            ILayerContainer layerContainer = layerConfigure.CreateLayerContainer();
            IViewLoader viewLoader = layerConfigure.CreateViewLoader();
            Type viewLocatorType = layerConfigure.GetViewLocatorType();
            layerContainer.Bind(layerLocator);
            layerLocator.Bind(this, viewLayer, layerContainer, viewLoader, viewLocatorType);
            layerLocators.Add(viewLayer, layerLocator);
        }
        viewService.SetLayerLocators(layerLocators);
    }
    private void CreateSubViewCollectLocators()
    {
        ISubViewCollectConfigures subViewCollectContainers = viewService.GetSubViewCollectConfigures();
        foreach (var pair in subViewCollectContainers)
        {
            SubViewCollect subViewCollect = pair.Key;
            ISubViewDisplayConfigure subViewDisplayConfigure = pair.Value;
            GameObject goLocator = new GameObject(subViewCollect.ToString());
            goLocator.transform.SetParent(rt);
            
            ISubViewCollectLocator subViewCollectLocator = subViewDisplayConfigure.AddSubViewDisplayLocator(goLocator);
            ISubViewCollectContainer subViewCollectContainer = subViewDisplayConfigure.CreateSubViewDisplayContainer();
            IViewLoader viewLoader = subViewDisplayConfigure.CreateViewLoader();
            Type subViewsLocatorType = subViewDisplayConfigure.GetSubViewsLocatorType();
            Type subViewLocatorType = subViewDisplayConfigure.GetSubViewLocatorType();
            subViewCollectContainer.Bind(subViewCollectLocator);
            subViewCollectLocator.Bind(this, subViewCollect, subViewCollectContainer, viewLoader, subViewsLocatorType, subViewLocatorType);
            subViewDisplayLocators.Add(subViewCollect, subViewCollectLocator);
        }
        viewService.SetSubViewCollectLocators(subViewDisplayLocators);
    }

    IViewService IUICanvasLocator.ViewService() => viewService;
    ILayerLocator IUICanvasLocator.GetLayerLocator(ViewLayer viewLayer) => layerLocators[viewLayer];
}
