using System;
using System.Collections.Generic;
using UnityEngine;

public class ViewConfigure: IViewConfigure
{
    private Type type;
    private IViewCheck viewCheck;
    private Type subViewsLocatorType;
    private List<ISubViewConfigure> subViewConfigures;

    private ViewLayer viewLayer;
    
    public ViewConfigure(Type type)
    {
        this.type = type;
    }
    public ViewConfigure(Type type, IViewCheck viewCheck)
    {
        this.type = type;
        this.viewCheck = viewCheck;
    }

    public ViewConfigure SubLayer<TSubViewsLocator>(List<ISubViewConfigure> subViewConfigures)
        where TSubViewsLocator: ISubViewLayerLocator
    {
        this.subViewsLocatorType = typeof(TSubViewsLocator);
        this.subViewConfigures = subViewConfigures;
        return this;
    }
    
    void IViewConfigure.AddViewLayer(ViewLayer viewLayer) => this.viewLayer = viewLayer;

    bool IViewConfigure.TryGetSubViewConfigures(out List<ISubViewConfigure> subViewConfigures)
    {
        if (this.subViewConfigures == null)
        {
            subViewConfigures = null;
            return false;
        }
        subViewConfigures = this.subViewConfigures;
        return true;
    }
    
    
    bool IViewConfigure.TryGetViewCheck(out IViewCheck viewCheck)
    {
        if (this.viewCheck == null)
        {
            viewCheck = null;
            return false;
        }
        viewCheck = this.viewCheck;
        return true;
    }

    bool IViewConfigure.TryGetSubViewCheck(SubViewShow subViewShow, out IViewCheck viewCheck)
    {
        if (subViewConfigures == null)
        {
            viewCheck = null;
            return false;
        }
        for (int i = 0; i < subViewConfigures.Count; i++)
        {
            ISubViewConfigure subViewConfigure = subViewConfigures[i];
            if (!subViewConfigure.EqualsSubViewShow(subViewShow)) continue;
            return subViewConfigure.TryGetViewCheck(out viewCheck);
        }
        viewCheck = null;
        return false;
    }

    ViewLayer IViewConfigure.GetViewLayer() => viewLayer;
    Type IViewConfigure.GetViewType() => type;
    
    ISubViewLayerLocator IViewConfigure.GetOrAddSubViewsLocator(GameObject goMainView)
    {
        ISubViewLayerLocator subViewLayerLocator = (ISubViewLayerLocator)goMainView.GetComponent(subViewsLocatorType);
        if (subViewLayerLocator == null)
        {
            subViewLayerLocator = (ISubViewLayerLocator)goMainView.AddComponent(subViewsLocatorType);
        }
        return subViewLayerLocator;
    }
}
