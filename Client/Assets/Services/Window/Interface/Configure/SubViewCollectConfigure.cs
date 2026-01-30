using System;
using UnityEngine;

public class SubViewCollectConfigure<TSubViewDisplayLocator, TSubViewDisplayContainer, TSubViewsLocator, TViewLoader, TSubViewLocator> : ISubViewDisplayConfigure
    where TSubViewDisplayLocator: MonoBehaviour, ISubViewCollectLocator
    where TSubViewDisplayContainer: ISubViewCollectContainer, new()
    where TSubViewsLocator: MonoBehaviour, ISubViewsLocator
    where TViewLoader: IViewLoader
    where TSubViewLocator: MonoBehaviour, IViewLocator
{
    private int poolCapacity;
    private int preDestroyCapacity;
    private int preDestroyMillisecondsDelay;

    public SubViewCollectConfigure(int poolCapacity, int preDestroyCapacity = 10, int preDestroyMillisecondsDelay = 10)
    {
        this.poolCapacity = poolCapacity;
        this.preDestroyCapacity = preDestroyCapacity;
        this.preDestroyMillisecondsDelay = preDestroyMillisecondsDelay;
    }

    ISubViewCollectLocator ISubViewDisplayConfigure.AddSubViewDisplayLocator(GameObject goSubViewDisplay)
    {
        return goSubViewDisplay.AddComponent<TSubViewDisplayLocator>();
    }
    ISubViewCollectContainer ISubViewDisplayConfigure.CreateSubViewDisplayContainer()
    {
        return new TSubViewDisplayContainer();
    }
    IViewLoader ISubViewDisplayConfigure.CreateViewLoader()
    {
        return (TViewLoader)Activator.CreateInstance
        (
            typeof(TViewLoader), poolCapacity, preDestroyCapacity, preDestroyMillisecondsDelay
        );
    }
    Type ISubViewDisplayConfigure.GetSubViewsLocatorType()
    {
        return typeof(TSubViewsLocator);
    }
    Type ISubViewDisplayConfigure.GetSubViewLocatorType()
    {
        return typeof(TSubViewLocator);
    }
}
