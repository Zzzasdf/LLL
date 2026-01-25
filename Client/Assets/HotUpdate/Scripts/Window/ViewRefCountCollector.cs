using System;
using System.Collections.Generic;

public class ViewRefCountCollector
{
    private Dictionary<Type, int> refCounts;

    public ViewRefCountCollector()
    {
        refCounts = new Dictionary<Type, int>();
    }

    public void RefIncrement(Type type)
    {
        refCounts.TryAdd(type, 0);
        refCounts[type] += 1;
    }
    public void RefReduction(Type type)
    {
        refCounts.TryAdd(type, 0);
        refCounts[type] -= 1;
    }
    public int GetRefCount(Type type)
    {
        refCounts.TryAdd(type, 0);
        return refCounts[type];
    }
}