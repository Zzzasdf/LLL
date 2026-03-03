using Cysharp.Threading.Tasks;
using UnityEngine;

public class EntityLoader_Unit<TKey, TValue> : IEntityLoader<TKey, TValue>
    where TValue : EntityBase
{
    private IEntityPool<TKey, TValue> entityPool;

    public EntityLoader_Unit(EntityPoolBase<TKey, TValue> entityPool)
    {
        this.entityPool = entityPool;
    }
    
    UniTask<TValue> IEntityLoader<TKey, TValue>.Get(TKey tKey)
    {
        return entityPool.Create(tKey);
    }

    void IEntityLoader<TKey, TValue>.Release(TKey tKey, TValue tValue)
    {
        Object.Destroy(tValue);
    }

    public void SetToPoolParent(TValue tValue)
    {
    }
}
