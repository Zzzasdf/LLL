using System;
using System.Collections.Generic;

public interface IViewConfigure
{
    void AddView<TView, TViewModel>()
        where TView : IView
        where TViewModel : class, IViewModel;
    void AddLayer(ViewLayer viewLayer);
    
    Dictionary<SubViewType, IViewCheck> GetSubViewTypes();
    
    Type GetViewType();
    IViewCheck GetViewCheck();
    ViewLayer GetViewLayer();
}
