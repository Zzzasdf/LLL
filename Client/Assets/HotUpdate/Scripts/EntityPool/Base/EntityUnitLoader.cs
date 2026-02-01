using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EntityUnitLoader<TKey, TValue> : IEntityLoader<TKey, TValue>
    where TValue : EntityBase
{
    private IEntityPool<TKey, TValue> entityPool;

    public EntityUnitLoader(EntityPoolBase<TKey, TValue> entityPool)
    {
        this.entityPool = entityPool;
    }
    
    bool IEntityLoader<TKey, TValue>.TryGetFromActive(TKey tKey, out TValue tValue)
    {
        tValue = null;
        return false;
    }

    bool IEntityLoader<TKey, TValue>.TryGetFromPool(TKey tKey, out TValue tValue)
    {
        tValue = null;
        return false;
    }

    UniTask<TValue> IEntityLoader<TKey, TValue>.Create(TKey tKey)
    {
        return entityPool.Create(tKey);
    }

    void IEntityLoader<TKey, TValue>.Release(TKey tKey, TValue tValue)
    {
        Object.Destroy(tValue);
    }

    List<int> IEntityLoader<TKey, TValue>.BatchAddFilter(List<TKey> types, List<int> uniqueIds)
    {
        return uniqueIds;
    }
}
