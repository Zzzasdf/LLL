using System;
using System.Collections.Generic;

public class SharedFactoryObjectPool : IFactoryObjectPool
{
    private readonly int capacity;
    private List<Type> sharedCapacity;
    private readonly Dictionary<Type, Queue<IReusable>> pools;

    public SharedFactoryObjectPool(int capacity = 0)
    {
        this.capacity = capacity;
        sharedCapacity = new List<Type>();
        pools = new Dictionary<Type, Queue<IReusable>>();
    }
    
    T IObjectPool.Get<T>()
    {
        Type type = typeof(T);
        if (!pools.TryGetValue(type, out Queue<IReusable> queue)
            || !queue.TryDequeue(out IReusable reusable))
        {
            reusable = new T();
            reusable.ReusableRecorder = new ReusableRecorder<T>();
        }
        else
        {
            sharedCapacity.Remove(type);
        }
        reusable.ReusableRecorder.ReRegisterForFinalize();
        return reusable as T;
    }
    IReusable IObjectPool.Get(Type reusableType)
    {
        if (!pools.TryGetValue(reusableType, out Queue<IReusable> queue)
            || !queue.TryDequeue(out IReusable reusable))
        {
            reusable = (IReusable)Activator.CreateInstance(reusableType);
            reusable.ReusableRecorder = new ReusableRecorder(reusableType);
        }
        else
        {
            sharedCapacity.Remove(reusableType);
        }
        reusable.ReusableRecorder.ReRegisterForFinalize();
        return reusable;
    }

    void IObjectPool.Release<T>(T toRelease)
    {
        if (toRelease.ReusableRecorder == null)
        {
            LLogger.FrameError($"该类型的创建并非来自对象池，不允许回收, {typeof(T)} : {toRelease}");
            return;
        }
        toRelease.ReusableRecorder.SuppressFinalize();
        Type type = typeof(T);
        if (sharedCapacity.Count == capacity && sharedCapacity.Count > 0)
        {
            Type oldestType = sharedCapacity[0];
            pools[oldestType].Dequeue();
            sharedCapacity.RemoveAt(0);
        }
        sharedCapacity.Add(type);
        if (!pools.TryGetValue(type, out Queue<IReusable> queue))
        {
            pools.Add(type, queue = new Queue<IReusable>());
        }
        queue.Enqueue(toRelease);
    }
    void IObjectPool.Release(IReusable toRelease)
    {
        Type type = toRelease.GetType();
        if (toRelease.ReusableRecorder == null)
        {
            LLogger.FrameError($"该类型的创建并非来自对象池，不允许回收, {type} : {toRelease}");
            return;
        }
        toRelease.ReusableRecorder.SuppressFinalize();
        if (sharedCapacity.Count == capacity && sharedCapacity.Count > 0)
        {
            Type oldestType = sharedCapacity[0];
            pools[oldestType].Dequeue();
            sharedCapacity.RemoveAt(0);
        }
        sharedCapacity.Add(type);
        if (!pools.TryGetValue(type, out Queue<IReusable> queue))
        {
            pools.Add(type, queue = new Queue<IReusable>());
        }
        queue.Enqueue(toRelease);
    }
}
