using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ViewDriver : ViewDriverBase
{
    protected RectTransform thisRt;
    protected Canvas canvas;
    protected GraphicRaycaster graphicRaycaster;
    protected CanvasScaler canvasScaler;

    protected override void Awake()
    { 
        base.Awake();
        thisRt = gameObject.GetComponent<RectTransform>();
        
        // 设置全屏拉伸
        thisRt.anchorMin = Vector2.zero;
        thisRt.anchorMax = Vector2.one;
        thisRt.offsetMin = Vector2.zero;
        thisRt.offsetMax = Vector2.zero;
        
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
        
        canvasScaler = gameObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.matchWidthOrHeight = 0.5f;
    }
}