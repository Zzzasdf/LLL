using System;

public interface IViewLayerDriverPool { }
public interface IViewLayerDriverLoader { }

public class ViewLayerDriverPool: EntityPool<ViewLayerDriverBase>, IViewLayerDriverPool
{
}
public class ViewLayerDriverLoader : EntityLoader<Type, ViewLayerDriverBase>, IViewLayerDriverLoader
{
    public ViewLayerDriverLoader(ViewLayerDriverPool entityPool) : base(entityPool)
    {
    }
}