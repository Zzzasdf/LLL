using System;

public interface IViewDriverPool { }
public interface IViewDriverLoader { }

public class ViewDriverPool: EntityPool<ViewDriverBase>, IViewDriverPool
{
}

public class ViewDriverLoader : EntityLoader<Type, ViewDriverBase>, IViewDriverLoader
{
    public ViewDriverLoader(ViewDriverPool entityPool) : base(entityPool)
    {
    }
}