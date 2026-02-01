using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class EntityMultipleLoader<TKey, TValue> : IEntityLoader<TKey, TValue>
    where TValue: EntityBase
{
    private IEntityPool<TKey, TValue> entityPool;

    public EntityMultipleLoader(EntityPoolBase<TKey, TValue> entityPool)
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
        return entityPool.TryGetFromPool(tKey, out tValue);
    }

    UniTask<TValue> IEntityLoader<TKey, TValue>.Create(TKey tKey)
    {
        return entityPool.Create(tKey);
    }

    void IEntityLoader<TKey, TValue>.Release(TKey tKey, TValue tValue)
    {
        EntityPoolBase<TKey, TValue> pool = (EntityPoolBase<TKey, TValue>)entityPool;
        tValue.transform.SetParent(pool.transform);
        entityPool.Release(tKey, tValue);
    }

    List<int> IEntityLoader<TKey, TValue>.BatchAddFilter(List<TKey> types, List<int> uniqueIds)
    {
        return uniqueIds;
    }
}
