using System;

public class ViewPool: EntityPool<ViewEntityBase>, IViewPool
{
}

public class ViewUnitLoader : EntityUnitLoader<Type, ViewEntityBase>, IViewLoader
{
    
    public ViewUnitLoader(ViewPool entityPool) : base(entityPool)
    {
    }
}

public class ViewUniqueLoader : EntityUniqueLoader<Type, ViewEntityBase>, IViewLoader
{
    public ViewUniqueLoader(ViewPool entityPool) : base(entityPool)
    {
    }
}

public class ViewMultipleLoader : EntityMultipleLoader<Type, ViewEntityBase>, IViewLoader
{
    public ViewMultipleLoader(ViewPool entityPool) : base(entityPool)
    {
    }
}