using System;
using System.Collections.Generic;

public interface IViewConfigure
{
    void AddLayer(ViewLayer viewLayer);
    bool TryGetSubViewConfigures(out List<ISubViewConfigure> subViewConfigures);

    bool TryGetViewCheck(out IViewCheck viewCheck);
    bool TryGetSubViewCheck(SubViewShow subViewShow, out IViewCheck viewCheck);

    ViewLayer GetViewLayer();
    Type GetViewType();
    
    SubViewCollect GetSubViewDisplay();
}
