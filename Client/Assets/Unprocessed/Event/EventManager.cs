using System;
using System.Collections.Generic;

public interface IEvent<in T>: IEvent
{
    void EventHandler(T data);
}
public interface IEvent
{
    
}
    
public class EventManager : IEventManager
{
    private Dictionary<Type, PooledHashSet<IEvent>> eventDict;

    void IInit.OnInit()
    {
        eventDict ??= new Dictionary<Type, PooledHashSet<IEvent>>();
    }
    void IReset.OnReset()
    {
        foreach (var pair in eventDict)
        {
            ((IDisposable)pair.Value).Dispose();
        }
        eventDict.Clear();
    }
    void IDestroy.OnDestroy()
    {
        foreach (var pair in eventDict)
        {
            ((IDisposable)pair.Value).Dispose();
        }
        eventDict.Clear();
    }
    
    public void Subscribe<T>(IEvent<T> tEvent)
    {
        Type type = typeof(T);
        if (!eventDict.TryGetValue(type, out PooledHashSet<IEvent> events))
        {
            eventDict.Add(type, events = PooledHashSet<IEvent>.Get());
        }
        events.Add(tEvent);
    }

    public void Unsubscribe<T>(IEvent<T> tEvent)
    {
        Type type = typeof(T);
        Unsubscribe(type, tEvent);
    }
    public void Unsubscribe(Type type, IEvent tEvent)
    {
        if (!eventDict.TryGetValue(type, out PooledHashSet<IEvent> events))
        {
            return;
        }
        events.Remove(tEvent);
        if (events.Count == 0)
        {
            ((IDisposable)events).Dispose();
            eventDict.Remove(type);
        }
    }

    public void FireNow<T>(T t)
    {
        Type type = typeof(T);
        if (!eventDict.TryGetValue(type, out PooledHashSet<IEvent> events))
        {
            return;
        }
        foreach (var item in events)
        {
            ((IEvent<T>)item).EventHandler(t);   
        }
    }
}

public class Events: IDisposable
{
    private Dictionary<Type, IEvent> eventDict;
    
    private static readonly MonitoredObjectPool.ObjectPool<Events, Events> s_Pool = 
        new("Events", () => new Events(), 
            null,
            l =>
            {
                l.OnUnAllSubscribe();
            });

    public static UnityEngine.Pool.PooledObject<Events> Get( out Events value)
    {
        UnityEngine.Pool.PooledObject<Events> result = s_Pool.Get(out value);
        return result;
    }

    public static Events Get()
    {
        Events result = s_Pool.Get();
        return result;
    }

    private Events()
    {
        eventDict = new Dictionary<Type, IEvent>();
    }
    void IDisposable.Dispose() => s_Pool.Release(this);

#if POOLED_EXCEPTION
    ~Events() => s_Pool.FinalizeDebug();
#endif

    public void Subscribe<T>(IEvent<T> tEvent)
    {
        Type type = typeof(T);
        eventDict.Add(type, tEvent);
        GameEntry.EventManager.Subscribe(tEvent);
    }
    public void OnUnAllSubscribe()
    {
        foreach (var pair in eventDict)
        {
            GameEntry.EventManager.Unsubscribe(pair.Key, pair.Value);
        }
        eventDict.Clear();
    }
}
