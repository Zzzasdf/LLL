using System;
using System.Collections.Generic;

public interface IViewConfigure
{
    public Type Type { get; }
    public ViewType ViewType { get; }
    public IViewCheck ViewCheck { get; }
    public Type ViewLoaderType { get; }
    public Type ViewDriverType { get; }
    
    public Type SubViewLayerCoreType { get; }
    public Type SubViewLayerDriverType { get; }
    public List<IViewConfigure> SubViewConfigures { get; }
}
