using System;
using UnityEngine;

public interface ISubViewCollectLocator
{
    void Bind(IUICanvasLocator uiCanvasLocator, SubViewCollect subViewCollect, ISubViewCollectContainer subViewCollectContainer, IViewLoader viewLoader, Type subViewsLocatorType, Type subViewLocatorType);
    ISubViewsLocator AddViewsLocator(IView view);

    IViewLoader GetViewLoader();
    IViewLocator AddSubViewLocator(GameObject goSubView);
}
