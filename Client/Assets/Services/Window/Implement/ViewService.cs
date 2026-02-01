using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using UnityEngine;
using UnityEngine.UI;

public partial class ViewService: MonoBehaviour, IViewService
{
    private GameObject go;
    private RectTransform thisRt;
    private Canvas canvas;
    private GraphicRaycaster graphicRaycaster;
    private CanvasScaler canvasScaler;
    
    private Dictionary<ViewLayer, IViewLayerLocator> viewLayerLocators;
    private Dictionary<Type, IViewConfigure> viewShows;
    private Dictionary<SubViewShow, IViewConfigure> subViewShows;

    [SerializeField] private ViewModelGenerator viewModelGenerator = ViewModelGenerator.Default;
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        
        go = gameObject;
        thisRt = go.AddComponent<RectTransform>();
        
        // 设置全屏拉伸
        thisRt.anchorMin = Vector2.zero;
        thisRt.anchorMax = Vector2.one;
        thisRt.offsetMin = Vector2.zero;
        thisRt.offsetMax = Vector2.zero;
        
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        graphicRaycaster = go.AddComponent<GraphicRaycaster>();
        
        canvasScaler = go.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.matchWidthOrHeight = 0.5f;
        
        viewLayerLocators = new Dictionary<ViewLayer, IViewLayerLocator>();
        viewShows = new Dictionary<Type, IViewConfigure>();
        subViewShows = new Dictionary<SubViewShow, IViewConfigure>();

        WeakReferenceMessenger.Default.Register<ViewShowAsyncRequestEvent>(this);
        WeakReferenceMessenger.Default.Register<ViewHideAsyncRequestEvent>(this);
        WeakReferenceMessenger.Default.Register<ViewAllHideAsyncRequestEvent>(this);
        WeakReferenceMessenger.Default.Register<ViewSubShowAsyncRequestEvent>(this);
    }
    
    void IViewService.Bind(Dictionary<ViewLayer, Type> layerLocators, Dictionary<ViewLayer, List<IViewConfigure>> configures)
    {
        foreach (var pair in configures)
        {
            ViewLayer viewLayer = pair.Key;
            List<IViewConfigure> viewConfigures = pair.Value;
            for (int i = 0; i < viewConfigures.Count; i++)
            {
                IViewConfigure viewConfigure = viewConfigures[i];
                viewConfigure.AddViewLayer(viewLayer);
                Type viewType = viewConfigure.GetViewType();
                viewShows.Add(viewType, viewConfigure);
                if (!viewConfigure.TryGetSubViewConfigures(out List<ISubViewConfigure> subViewConfigures))
                {
                    continue;
                }
                for (int j = 0; j < subViewConfigures.Count; j++)
                {
                    ISubViewConfigure subViewConfigure = subViewConfigures[j];
                    if (!subViewConfigure.TryGetSubViewShow(out SubViewShow subViewShow))
                    {
                        continue;
                    }
                    subViewShows.Add(subViewShow, viewConfigure);
                }
            }
        }
        foreach (var pair in layerLocators)
        {
            ViewLayer viewLayer = pair.Key;
            Type layerLocator = pair.Value;
            GameObject goLayer = new GameObject(viewLayer.ToString());
            goLayer.transform.SetParent(transform);
            IViewLayerLocator viewLayerLocator = (IViewLayerLocator)goLayer.AddComponent(layerLocator);
            List<IViewConfigure> viewConfigures = configures[viewLayer];
            viewLayerLocator.Init(viewLayer, viewConfigures);
            viewLayerLocators.Add(viewLayer, viewLayerLocator);
        }
    }
}