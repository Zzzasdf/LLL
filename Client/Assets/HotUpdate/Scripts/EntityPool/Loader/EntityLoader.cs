using Cysharp.Threading.Tasks;

public class EntityLoader<TKey, TValue> : IEntityLoader<TKey, TValue>
    where TValue: EntityBase
{
    private IEntityPool<TKey, TValue> entityPool;

    public EntityLoader(EntityPoolBase<TKey, TValue> entityPool)
    {
        this.entityPool = entityPool;
    }
    async UniTask<TValue> IEntityLoader<TKey, TValue>.Get(TKey tKey)
    {
        if (entityPool.TryGetFromPool(tKey, out TValue tValue))
        {
            tValue.gameObject.SetActive(true);
            return tValue;
        }
        return await entityPool.Create(tKey);
    }

    void IEntityLoader<TKey, TValue>.Release(TKey tKey, TValue tValue)
    {
        entityPool.Release(tKey, tValue);
    }
}
