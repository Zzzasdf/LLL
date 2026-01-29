using System;
using System.Collections.Generic;

public interface ISubViewConfigure
{
    void AddSubViewContainerType(SubViewContainerType subViewContainerType);
    
    Type GetSubViewType();
    List<SubViewAKA> GetSubViewAKAs();
    bool IsFuncOpen();
    bool IsFuncOpenWithTip();
    SubViewContainerType GetSubViewContainerType();
}
