using System;
using System.Collections.Generic;

public interface ISubViewConfigure
{
    void AddView<TView, TViewModel>(List<SubViewType> subViewTypes) 
        where TView : IView
        where TViewModel : class, IViewModel;
    
    Type GetSubViewType();
    List<SubViewType> GetSubViewTypes();
}
