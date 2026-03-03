using System;
using System.Collections.Generic;

public class ViewConfigure: IViewConfigure
{
    public Type Type { get; }
    public ViewType ViewType { get; }
    public IViewCheck ViewCheck { get; }
    public Type ViewLoaderType { get; }
    public Type ViewDriverType { get; }
    
    public Type SubViewLayerCoreType { get; }
    public Type SubViewLayerDriverType { get; }
    public List<IViewConfigure> SubViewConfigures { get; }

    public ViewConfigure(Type type, ViewType viewType, IViewCheck viewCheck, Type viewLoaderType, Type viewDriverType, Type viewLayerCoreType, Type viewLayerDriverType, List<IViewConfigure> subViewConfigures) 
    {
        Type = type;
        ViewType = viewType;
        ViewCheck = viewCheck;
        ViewLoaderType = viewLoaderType;
        ViewDriverType = viewDriverType;
        SubViewLayerCoreType = viewLayerCoreType;
        SubViewLayerDriverType = viewLayerDriverType;
        SubViewConfigures = subViewConfigures;
    }
}
