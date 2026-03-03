using System;
using System.Collections.Generic;

public class PoolService : IPoolService
{
    private Dictionary<Type, IObjectPool> pools;
    
    public PoolService(Dictionary<IFactoryObjectPool, List<Type>> customPools): this()
    {
        if (customPools == null) return;
        foreach (var pair in customPools)
        {
            IFactoryObjectPool objectPool = pair.Key;
            List<Type> types = pair.Value;
            for (int i = 0; i < types.Count; i++)
            {
                Type type = types[i];
                pools.Add(type, objectPool);
            }
        }
    }
    public PoolService()
    {
        pools = new Dictionary<Type, IObjectPool>();
    }

    T IPoolService.Get<T>()
    {
        Type type = typeof(T);
        if (!pools.TryGetValue(type, out IObjectPool objectPool))
        {
            pools.Add(type, objectPool = new ObjectPool());
        }
        return objectPool.Get<T>();
    }
    IReusable IPoolService.Get(Type reusableType)
    {
        if (!pools.TryGetValue(reusableType, out IObjectPool objectPool))
        {
            pools.Add(reusableType, objectPool = new ObjectPool());
        }
        return objectPool.Get(reusableType);
    }

    void IPoolService.Release<T>(T toRelease)
    {
        Type type = typeof(T);
        if (!pools.TryGetValue(type, out IObjectPool objectPool))
        {
            pools.Add(type, objectPool = new ObjectPool());
        }
        objectPool.Release(toRelease);
    }

    void IPoolService.Release(IReusable toRelease)
    {
        Type type = toRelease.GetType();
        if (!pools.TryGetValue(type, out IObjectPool objectPool))
        {
            pools.Add(type, objectPool = new ObjectPool());
        }
        objectPool.Release(toRelease);
    }
}
