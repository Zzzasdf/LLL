using System;
using System.Collections.Generic;
using JetBrains.Annotations;

public class ViewCheckGenerator
{
    private static ViewCheckGenerator _default = new ViewCheckGenerator();
    public static ViewCheckGenerator Default => _default;
    
    private Dictionary<int, IViewCheck> uniqueIdChecks;

    private Dictionary<Type, IViewCheck> viewChecks;
    private Dictionary<SubViewType, IViewCheck> subViewChecks;
    
    private ViewCheckGenerator()
    {
        uniqueIdChecks = new Dictionary<int, IViewCheck>();
    }
    public void Assembly(Dictionary<Type, IViewCheck> viewChecks, Dictionary<SubViewType, IViewCheck> subViewChecks)
    {
        this.viewChecks = viewChecks;
        this.subViewChecks = subViewChecks;
    }
    
    public void Add(Type type, int uniqueId)
    {
        if (uniqueIdChecks.TryGetValue(uniqueId, out IViewCheck viewCheck)) return;
        if (!viewChecks.TryGetValue(type, out viewCheck)) return;
        uniqueIdChecks.Add(uniqueId, viewCheck);
    }
    public void Add(SubViewType subViewType, int uniqueId)
    {
        if (uniqueIdChecks.TryGetValue(uniqueId, out IViewCheck viewCheck)) return;
        if (!subViewChecks.TryGetValue(subViewType, out viewCheck)) return;
        uniqueIdChecks.Add(uniqueId, viewCheck);
    }
    [CanBeNull]
    public IViewCheck Get(int uniqueId)
    {
        if (!uniqueIdChecks.TryGetValue(uniqueId, out IViewCheck viewCheck))
        {
            return null;
        }
        return viewCheck;
    }
    
    public bool Delete(int uniqueId)
    {
        if (!uniqueIdChecks.Remove(uniqueId))
        {
            LLogger.Error($"不存在 UniqueId: {uniqueId} 映射的 {nameof(IViewCheck)}");
            return false;
        }
        return true;
    }
}