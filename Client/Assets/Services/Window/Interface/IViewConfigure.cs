using System;
using System.Collections.Generic;

public interface IViewConfigure
{
    void Build(ViewLayer viewLayer);
    List<Type> GetSubViewTypes();
    List<SubViewAKA> GetSubViewAKAs();
    
    Type GetViewType();
    bool IsFuncOpen();
    bool IsFuncOpenWithTip();
    ViewLayer GetViewLayer();
}
