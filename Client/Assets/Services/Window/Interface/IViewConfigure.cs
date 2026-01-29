using System;
using System.Collections.Generic;

public interface IViewConfigure
{
    void AddLayer(ViewLayer viewLayer);
    
    List<Type> GetSubViewTypes();
    List<SubViewAKA> GetSubViewAKAs();
    
    Type GetViewType();
    bool IsFuncOpen();
    bool IsFuncOpenWithTip();
    ViewLayer GetViewLayer();
}
