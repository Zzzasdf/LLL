using System;
using System.Collections.Generic;

public class ObjectPool : IObjectPool
{
    private readonly int capacity;
    private readonly Queue<IReusable> pools;

    public ObjectPool(int capacity = 0)
    {
        this.capacity = capacity;
        pools = new Queue<IReusable>();
    }
    
    T IObjectPool.Get<T>()
    {
        if (!pools.TryDequeue(out IReusable reusable))
        {
            reusable = new T();
            reusable.ReusableRecorder = new ReusableRecorder<T>();
        }
        reusable.ReusableRecorder.ReRegisterForFinalize();
        return reusable as T;
    }
    IReusable IObjectPool.Get(Type reusableType)
    {
        if (!pools.TryDequeue(out IReusable reusable))
        {
            reusable = (IReusable)Activator.CreateInstance(reusableType);
            reusable.ReusableRecorder = new ReusableRecorder(reusableType);
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
        pools.Enqueue(toRelease);
        if (pools.Count == capacity)
        {
            pools.TryDequeue(out _);
        }
        pools.Enqueue(toRelease);
    }
    void IObjectPool.Release(IReusable toRelease)
    {
        if (toRelease.ReusableRecorder == null)
        {
            LLogger.FrameError($"该类型的创建并非来自对象池，不允许回收, {toRelease.GetType()} : {toRelease}");
            return;
        }
        toRelease.ReusableRecorder.SuppressFinalize();
        pools.Enqueue(toRelease);
        if (pools.Count == capacity)
        {
            pools.TryDequeue(out _);
        }
        pools.Enqueue(toRelease);
    }
}
