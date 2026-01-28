using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;

public class ObjectPoolAsync<TKey, TValue>
{
    private readonly int poolCapacity;
    private readonly List<TKey> poolIteration;
    private readonly Dictionary<TKey, Queue<TValue>> pools;

    private readonly int preDestroyCapacity;
    private readonly int preDestroyMillisecondsDelay;
    private readonly List<TKey> preDestroyIteration;
    private readonly Dictionary<TKey, Queue<(TValue, CancellationTokenSource)>> preDestroys;
    
    private readonly Func<TKey, UniTask<TValue>> createFunc;
    private readonly Action<TValue> destroyFunc;
    
    protected ObjectPoolAsync(int poolCapacity, int preDestroyCapacity, int preDestroyMillisecondsDelay, 
        Func<TKey, UniTask<TValue>> createFunc,
        Action<TValue> destroyFunc)
    {
        this.poolCapacity = poolCapacity;
        poolIteration = new List<TKey>();
        pools = new Dictionary<TKey, Queue<TValue>>();

        this.preDestroyCapacity = preDestroyCapacity;
        this.preDestroyMillisecondsDelay = preDestroyMillisecondsDelay;
        preDestroyIteration = new List<TKey>();
        preDestroys = new Dictionary<TKey, Queue<(TValue, CancellationTokenSource)>>();
        
        this.createFunc = createFunc;
        this.destroyFunc = destroyFunc;
    }

    protected async UniTask<TValue> Get(TKey tKey)
    {
        if (TryGetFromPool(tKey, out TValue tValue))
        {
            return tValue;
        }
        tValue = await Create(tKey);
        return tValue;
    }
    protected bool TryGetFromPool(TKey tKey, out TValue tValue)
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
    protected async UniTask<TValue> Create(TKey tKey)
    {
        return await createFunc.Invoke(tKey);
    }

    protected void Release(TKey tKey, TValue tValue)
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
                destroyFunc.Invoke(removeValue);
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
            destroyFunc.Invoke(tDestroyValue);
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
            destroyFunc.Invoke(tDestroyValue);
            LLogger.FrameLog($"await destroy {tDestroyValue.GetType().Name}");
        }
    }

    public override string ToString()
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
