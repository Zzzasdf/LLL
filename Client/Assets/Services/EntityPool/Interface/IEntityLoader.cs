using Cysharp.Threading.Tasks;

public interface IEntityLoader<TKey, TValue>: IEntityLoader
{
    UniTask<TValue> Get(TKey tKey);
    void Release(TKey tKey, TValue tValue);
    string ToString();
}

public interface IEntityLoader
{
    
}