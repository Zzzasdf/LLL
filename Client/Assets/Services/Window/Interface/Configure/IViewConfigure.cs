using System;
using System.Collections.Generic;
using UnityEngine;

public interface IViewConfigure
{
    void AddViewLayer(ViewLayer viewLayer);
    bool TryGetSubViewConfigures(out List<ISubViewConfigure> subViewConfigures);

    bool TryGetViewCheck(out IViewCheck viewCheck);
    bool TryGetSubViewCheck(SubViewShow subViewShow, out IViewCheck viewCheck);

    ViewLayer GetViewLayer();
    Type GetViewType();
    
    ISubViewLayerLocator GetOrAddSubViewsLocator(GameObject goMainView);
}
