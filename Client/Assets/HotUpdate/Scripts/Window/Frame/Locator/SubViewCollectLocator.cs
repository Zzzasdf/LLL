using System;
using UnityEngine;

public class SubViewCollectLocator : MonoBehaviour, ISubViewCollectLocator
{
    private IUICanvasLocator uiCanvasLocator;
    private SubViewCollect subViewCollect;
    private ISubViewCollectContainer subViewCollectContainer;
    private IViewLoader viewLoader;
    private Type subViewsLocatorType;
    private Type subViewLocatorType;
    private RectTransform thisRt;
    
    public void Build(ISubViewCollectContainer subViewCollectContainer, IUICanvasLocator uiCanvasLocator)
    {
        this.subViewCollectContainer = subViewCollectContainer;
        
        this.uiCanvasLocator = uiCanvasLocator;
        thisRt = gameObject.GetComponent<RectTransform>();
        if (thisRt == null)
        {
            thisRt = gameObject.AddComponent<RectTransform>();
        }
    }

    void ISubViewCollectLocator.Bind(IUICanvasLocator uiCanvasLocator, SubViewCollect subViewCollect, ISubViewCollectContainer subViewCollectContainer, IViewLoader viewLoader, Type subViewsLocatorType, Type subViewLocatorType)
    {
        this.uiCanvasLocator = uiCanvasLocator;
        this.subViewCollect = subViewCollect;
        this.subViewCollectContainer = subViewCollectContainer;
        this.viewLoader = viewLoader;
        this.subViewsLocatorType = subViewsLocatorType;
        this.subViewLocatorType = subViewLocatorType;
        thisRt = gameObject.GetComponent<RectTransform>();
        if (thisRt == null)
        {
            thisRt = gameObject.AddComponent<RectTransform>();
        }
    }

    ISubViewsLocator ISubViewCollectLocator.AddViewsLocator(IView view)
    {
        return (ISubViewsLocator)view.GameObject().AddComponent(subViewsLocatorType);
    }
    IViewLoader ISubViewCollectLocator.GetViewLoader()
    {
        return viewLoader;
    }

    IViewLocator ISubViewCollectLocator.AddSubViewLocator(GameObject goSubView)
    {
        return (IViewLocator)goSubView.AddComponent(subViewLocatorType);
    }
}
