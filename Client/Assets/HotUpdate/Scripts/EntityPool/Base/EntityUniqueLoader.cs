using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;

public class EntityUniqueLoader<TKey, TValue> : IEntityLoader<TKey, TValue>
    where TValue: EntityBase
{
    private IEntityPool<TKey, TValue> entityPool;
    private Dictionary<TKey, TValue> actives;

    public EntityUniqueLoader(EntityPoolBase<TKey, TValue> entityPool)
    {
        this.entityPool = entityPool;
        this.actives = new Dictionary<TKey, TValue>();
    }

    bool IEntityLoader<TKey, TValue>.TryGetFromActive(TKey tKey, out TValue tValue)
    {
        return actives.TryGetValue(tKey, out tValue);
    }

    bool IEntityLoader<TKey, TValue>.TryGetFromPool(TKey tKey, out TValue tValue)
    {
        if (entityPool.TryGetFromPool(tKey, out tValue))
        {
            actives.Add(tKey, tValue);
            return true;
        }
        return false;
    }
    
    async UniTask<TValue> IEntityLoader<TKey, TValue>.Create(TKey tKey)
    {
        TValue view = await entityPool.Create(tKey);
        actives.Add(tKey, view);
        return view;
    }
    
    void IEntityLoader<TKey, TValue>.Release(TKey tKey, TValue tValue)
    {
        EntityPoolBase<TKey, TValue> pool = (EntityPoolBase<TKey, TValue>)entityPool;
        tValue.transform.SetParent(pool.transform);
        actives.Remove(tKey);
        entityPool.Release(tKey, tValue);
    }

    List<int> IEntityLoader<TKey, TValue>.BatchAddFilter(List<TKey> tKeys, List<int> uniqueIds)
    {
        List<TKey> newTKeys = new List<TKey>();
        List<int> newUniqueIds = new List<int>();
        for (int i = 0; i < tKeys.Count; i++)
        {
            TKey tKey = tKeys[i];
            int uniqueId = uniqueIds[i];
            if (newTKeys.Contains(tKey))
            {
                int index = newTKeys.IndexOf(tKey);
                newTKeys.RemoveAt(index);
                newUniqueIds.RemoveAt(index);
            }
            newTKeys.Add(tKey);
            newUniqueIds.Add(uniqueId);
        }
        return newUniqueIds;
    }

    string IEntityLoader<TKey, TValue>.ToString()
    {
        string baseString = base.ToString();
        StringBuilder sb = new StringBuilder(entityPool.ToString());
        int index = 0;
        sb.AppendLine("actives => ");
        foreach (var pair in actives)
        {
            sb.AppendLine($"[{index}] => key: {pair.Key}, value: {pair.Value}");
            index++;
        }
        return $"{baseString}{sb}";
    }
}
