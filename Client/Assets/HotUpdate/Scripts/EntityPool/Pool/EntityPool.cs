using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class EntityPool<TValue> : EntityPoolBase<Type, TValue>
    where TValue: EntityBase
{
    protected override string GetName(Type type) => type.Name;
}

public abstract class EntityPoolBase<TKey, TValue> : MonoBehaviour, IEntityPool<TKey, TValue>
    where TValue: EntityBase
{
    private int poolCapacity;
    private List<TKey> poolIteration;
    private Dictionary<TKey, Queue<TValue>> pools;

    private int preDestroyCapacity;
    private int preDestroyMillisecondsDelay;
    private List<TKey> preDestroyIteration;
    private Dictionary<TKey, Queue<(TValue, CancellationTokenSource)>> preDestroys;

    private IEntityPool<TKey, TValue> entityPool;

    void IEntityPool.Init(int poolCapacity, int preDestroyCapacity, int preDestroyMillisecondsDelay)
    {
        this.poolCapacity = poolCapacity;
        this.poolIteration = new List<TKey>();
        this.pools = new Dictionary<TKey, Queue<TValue>>();

        this.preDestroyCapacity = preDestroyCapacity;
        this.preDestroyMillisecondsDelay = preDestroyMillisecondsDelay;
        this.preDestroyIteration = new List<TKey>();
        this.preDestroys = new Dictionary<TKey, Queue<(TValue, CancellationTokenSource)>>();

        entityPool = this;
    }

    async UniTask<TValue> IEntityPool<TKey, TValue>.Get(TKey tKey)
    {
        if (entityPool.TryGetFromPool(tKey, out TValue tValue))
        {
            return tValue;
        }
        tValue = await entityPool.Create(tKey);
        return tValue;
    }

    bool IEntityPool<TKey, TValue>.TryGetFromPool(TKey tKey, out TValue tValue)
    {
        if (preDestroys.TryGetValue(tKey, out Queue<(TValue, CancellationTokenSource)> preDestroyQueue)
            && preDestroyQueue.Count > 0)
        {
            preDestroyIteration.Remove(tKey);
            preDestroyQueue.TryDequeue(out (TValue tPreDestroyValue, CancellationTokenSource cts) item);
            item.cts.Cancel();
            item.cts.Dispose();
            tValue = item.tPreDestroyValue;
            return true;
        }
        if (pools.TryGetValue(tKey, out Queue<TValue> queue)
            && queue.Count > 0)
        {
            poolIteration.Remove(tKey);
            tValue = queue.Dequeue();
            return true;
        }
        tValue = default;
        return false;
    }
    
    protected abstract string GetName(TKey tKey);
    
    async UniTask<TValue> IEntityPool<TKey, TValue>.Create(TKey tKey)
    {
        string name = GetName(tKey);
        var handle = YooAssets.LoadAssetAsync<GameObject>(name);
        await handle.Task;
        if (handle.Status != EOperationStatus.Succeed)
        {
            LLogger.FrameError($"未找到该资源 {name}");
            return default;
        }
        GameObject assetObject = handle.AssetObject as GameObject;
        if (assetObject == null)
        {
            LLogger.FrameError($"加载的资源不是 GameObject: {name}");
            return default;
        }
        GameObject instantiatedObject = UnityEngine.Object.Instantiate(assetObject);
        if (instantiatedObject == null)
        {
            LLogger.FrameError($"实例化失败: {name}");
            return default;
        }
        TValue tValue = instantiatedObject.GetComponent<TValue>();
        return tValue;
    }

    void IEntityPool<TKey, TValue>.Release(TKey tKey, TValue tValue)
    {
        poolIteration.Add(tKey);
        if (!pools.TryGetValue(tKey, out Queue<TValue> queue))
        {
            pools.Add(tKey, queue = new Queue<TValue>());
        }
        queue.Enqueue(tValue);
        if (poolIteration.Count > poolCapacity)
        {
            TKey removeKey = poolIteration[0];
            poolIteration.RemoveAt(0);
            Queue<TValue> removeQueue = pools[removeKey];
            TValue removeValue = removeQueue.Dequeue();
            if (preDestroyCapacity <= 0 || preDestroyMillisecondsDelay <= 0)
            {
                Destroy(removeValue);
            }
            else
            {
                PushPreDestroy(removeKey, removeValue).Forget();
            }
        }
    }
    private async UniTask PushPreDestroy(TKey tKey, TValue tValue)
    {
        preDestroyIteration.Add(tKey);
        if (!preDestroys.TryGetValue(tKey, out Queue<(TValue, CancellationTokenSource)> queue))
        {
            preDestroys.Add(tKey, queue = new Queue<(TValue, CancellationTokenSource)>());
        }
        CancellationTokenSource cts = new CancellationTokenSource();
        queue.Enqueue((tValue, cts));
        if (preDestroyIteration.Count > preDestroyCapacity)
        {
            TKey removeKey = preDestroyIteration[0];
            preDestroyIteration.RemoveAt(0);
            Queue<(TValue, CancellationTokenSource)> preDestroyQueue = preDestroys[removeKey];
            (TValue tDestroyValue, CancellationTokenSource destroyCts) = preDestroyQueue.Dequeue();
            destroyCts.Cancel();
            destroyCts.Dispose();
            Destroy(tDestroyValue);
            LLogger.FrameLog($"destroy {tDestroyValue.GetType().Name}");
        }
        if(preDestroyIteration.Count == 0) return;
        await UniTask.Delay(preDestroyMillisecondsDelay, cancellationToken: cts.Token);
        {
            TKey removeKey = preDestroyIteration[0];
            preDestroyIteration.RemoveAt(0);
            Queue<(TValue, CancellationTokenSource)> preDestroyQueue = preDestroys[removeKey];
            (TValue tDestroyValue, CancellationTokenSource destroyCts) = preDestroyQueue.Dequeue();
            destroyCts.Dispose();
            Destroy(tDestroyValue);
            LLogger.FrameLog($"await destroy {tDestroyValue.GetType().Name}");
        }
    }

    string IEntityPool<TKey, TValue>.ToString()
    {
        StringBuilder sb = new StringBuilder(GetType().Name);
        sb.AppendLine($"pool capacity => {poolCapacity}");
        int index = 0;
        sb.AppendLine($"pool iteration => ");
        foreach (var item in poolIteration)
        {
            sb.AppendLine($"[{index}] => {item}");
            index++;
        }
        index = 0;
        sb.AppendLine("pools => ");
        foreach (var pair in pools)
        {
            int queueIndex = 0;
            StringBuilder sbQueue = new StringBuilder();
            foreach (var uniqueId in pair.Value)
            {
                sbQueue.Append($"[{queueIndex}] => {uniqueId}");
                queueIndex++;
            }
            sb.AppendLine($"[{index}] => key: {pair.Key}, value: {sbQueue}");
            index++;
        }
        
        sb.AppendLine();
        sb.AppendLine($"preDestroy capacity => {preDestroyCapacity}");
        index = 0;
        sb.AppendLine($"preDestroy iteration => ");
        foreach (var item in preDestroyIteration)
        {
            sb.AppendLine($"[{index}] => {item}");
            index++;
        }
        index = 0;
        sb.AppendLine("preDestroys => ");
        foreach (var pair in preDestroys)
        {
            int queueIndex = 0;
            StringBuilder sbQueue = new StringBuilder();
            foreach (var uniqueId in pair.Value)
            {
                sbQueue.Append($"[{queueIndex}] => {uniqueId}");
                queueIndex++;
            }
            sb.AppendLine($"[{index}] => key: {pair.Key}, value: {sbQueue}");
            index++;
        }
        return sb.ToString();
    }
}
