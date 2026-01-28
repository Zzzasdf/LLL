using System;
using System.Collections.Generic;

public interface ISubViewConfigure
{
    Type GetSubViewType();
    List<SubViewAKA> GetSubViewAKAs();
    
    bool IsFuncOpen();
    bool IsFuncOpenWithTip();
}
