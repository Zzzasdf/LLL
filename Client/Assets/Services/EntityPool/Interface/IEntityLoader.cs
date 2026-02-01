using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface IEntityLoader<TKey, TValue>
{
    bool TryGetFromActive(TKey tKey, out TValue tValue);
    bool TryGetFromPool(TKey tKey, out TValue tValue);
    UniTask<TValue> Create(TKey tKey);
    void Release(TKey tKey, TValue tValue);
    
    List<int> BatchAddFilter(List<TKey> types, List<int> uniqueIds);

    string ToString();
}
