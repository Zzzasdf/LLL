using System;
using UnityEngine;

public class LayerConfigure<TLayerLocator, TLayerContainer, TViewLoader, TViewLocator>: ILayerConfigure
    where TLayerLocator: MonoBehaviour, ILayerLocator
    where TLayerContainer : ILayerContainer, new()
    where TViewLocator: MonoBehaviour, IViewLocator
    where TViewLoader: IViewLoader
{
    private int poolCapacity;
    private int preDestroyCapacity;
    private int preDestroyMillisecondsDelay;

    public LayerConfigure(int poolCapacity, int preDestroyCapacity = 10, int preDestroyMillisecondsDelay = 10)
    {
        this.poolCapacity = poolCapacity;
        this.preDestroyCapacity = preDestroyCapacity;
        this.preDestroyMillisecondsDelay = preDestroyMillisecondsDelay;
    }

    ILayerLocator ILayerConfigure.AddLayerLocator(GameObject goLayer)
    {
        return goLayer.AddComponent<TLayerLocator>();
    }
    ILayerContainer ILayerConfigure.CreateLayerContainer()
    {
        return new TLayerContainer();
    }
    IViewLoader ILayerConfigure.CreateViewLoader()
    {
        return (TViewLoader)Activator.CreateInstance
        (
            typeof(TViewLoader), poolCapacity, preDestroyCapacity, preDestroyMillisecondsDelay
        );
    }
    Type ILayerConfigure.GetViewLocatorType()
    {
        return typeof(TViewLocator);
    }
}
