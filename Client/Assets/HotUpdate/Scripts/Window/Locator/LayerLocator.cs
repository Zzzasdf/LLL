using UnityEngine;

public class LayerLocator : MonoBehaviour
{
    private ViewLayer viewLayer;
    private ILayerContainer layerContainer;

    private RectTransform thisRt;
    
    public void Build(ViewLayer viewLayer)
    {
        this.viewLayer = viewLayer;
        thisRt = gameObject.AddComponent<RectTransform>();
        
        // 设置全屏拉伸
        thisRt.anchorMin = Vector2.zero;
        thisRt.anchorMax = Vector2.one;
        thisRt.offsetMin = Vector2.zero;
        thisRt.offsetMax = Vector2.zero;
        
        // 添加CanvasGroup用于控制整组UI
        CanvasGroup canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void BindContainer(ILayerContainer layerContainer)
    {
        this.layerContainer = layerContainer;
    }

    public RectTransform GetRectTransform() => thisRt;
}
