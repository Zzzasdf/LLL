using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasLocator : MonoBehaviour, IUICanvasLocator
{
    private IViewService viewService;
    private Dictionary<ViewLayer, ILayerLocator> layerLocators;

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
    }

    private void CreateLayerLocators()
    {
        Dictionary<ViewLayer, ILayerContainer> layerContainers = viewService.GetLayerContainers();
        foreach (var pair in layerContainers)
        {
            ViewLayer viewLayer = pair.Key;
            var layerContainer = pair.Value;
            GameObject goLocator = new GameObject(viewLayer.ToString());
            goLocator.transform.SetParent(rt);
            
            var layerLocator = layerContainer.AddLocator(goLocator);
            layerLocator.Build(layerContainer, this);
            layerLocators.Add(viewLayer, layerLocator);
        }
    }

    IViewService IUICanvasLocator.ViewService() => viewService;
    ILayerLocator IUICanvasLocator.GetLayerLocator(ViewLayer viewLayer) => layerLocators[viewLayer];
}
