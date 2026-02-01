using System;
using UnityEngine;
using UnityEngine.UI;

public class ViewLayerRaycastBlockingLocatorBase<TViewLayerContainer, TViewLoader, TViewLocator> : ViewLayerUnitLocatorBase<TViewLayerContainer, TViewLoader, TViewLocator>
    where TViewLayerContainer: class, IViewLayerContainer
    where TViewLoader: class, IEntityLoader<Type, ViewEntityBase>, IViewLoader
    where TViewLocator: MonoBehaviour, IViewLocator
{
    protected GameObject goMask;
    protected RectTransform rtMask;
    protected Image imgMask;
    
    protected override void Awake()
    {
        base.Awake();
        goMask = new GameObject("LayerMask");
        goMask.SetActive(false);
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
    
    protected override void CheckUniqueViewDictCount(int count)
    {
        goMask.SetActive(count > 0);
    }
}
