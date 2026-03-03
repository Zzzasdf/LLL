using System;

public interface IViewPool { }
public interface IViewLoader { }

public class ViewPool: EntityPool<ViewEntityBase>, IViewPool
{
}

public class ViewLoader : EntityLoader<Type, ViewEntityBase>, IViewLoader
{
    public ViewLoader(ViewPool entityPool) : base(entityPool)
    {
    }
}

public class ViewLoaderUnit : EntityLoader<Type, ViewEntityBase>, IViewLoader
{
    public ViewLoaderUnit(ViewPool entityPool) : base(entityPool)
    {
    }
}

// public class ViewUniqueLoader : EntityUniqueLoader<Type, ViewEntityBase>, IViewLoader
// {
//     public ViewUniqueLoader(ViewPool entityPool) : base(entityPool)
//     {
//     }
// }