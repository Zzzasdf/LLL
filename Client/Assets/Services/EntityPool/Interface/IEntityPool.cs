using Cysharp.Threading.Tasks;

public interface IEntityPool<TKey, TValue>: IEntityPool
{
    UniTask<TValue> Get(TKey tKey);
    bool TryGetFromPool(TKey tKey, out TValue tValue);
    UniTask<TValue> Create(TKey tKey);
    void Release(TKey tKey, TValue tValue);
    string ToString();
}
public interface IEntityPool
{
    void Init(int poolCapacity, int preDestroyCapacity, int preDestroyMillisecondsDelay);
}