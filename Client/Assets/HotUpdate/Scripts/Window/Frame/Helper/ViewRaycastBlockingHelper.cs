using UnityEngine;
using UnityEngine.UI;

public class ViewRaycastBlockingHelper : ViewUnitHelper
{
    protected GameObject goMask;
    protected RectTransform rtMask;
    protected Image imgMask;
    
    protected override void Awake()
    {
        base.Awake();
        
        goMask = new GameObject("Mask");
        goMask.transform.SetParent(transform);
        goMask.transform.SetAsFirstSibling();
        rtMask = goMask.AddComponent<RectTransform>();
        rtMask.localPosition = Vector3.zero;
        rtMask.localScale = Vector3.one;
        rtMask.anchoredPosition = Vector2.zero;
        rtMask.anchorMin = Vector2.zero;
        rtMask.anchorMax = Vector2.one;
        rtMask.offsetMin = Vector2.zero;
        rtMask.offsetMax = Vector2.zero;
        
        imgMask = goMask.AddComponent<Image>();
        imgMask.color = new Color(0, 0, 0, 0);
    }
}
