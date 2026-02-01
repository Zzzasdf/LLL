using System;

public class SubViewPool: EntityPool<ViewEntityBase>, ISubViewPool
{
    
}

public class SubViewUnitLoader : EntityUnitLoader<Type, ViewEntityBase>, ISubViewLoader
{
    public SubViewUnitLoader(SubViewPool entityPool) : base(entityPool)
    {
    }
}

public class SubViewUniqueLoader : EntityUniqueLoader<Type, ViewEntityBase>, ISubViewLoader
{
    public SubViewUniqueLoader(SubViewPool entityPool) : base(entityPool)
    {
    }
}

public class SubViewMultipleLoader : EntityMultipleLoader<Type, ViewEntityBase>, ISubViewLoader
{
    public SubViewMultipleLoader(SubViewPool entityPool) : base(entityPool)
    {
    }
}