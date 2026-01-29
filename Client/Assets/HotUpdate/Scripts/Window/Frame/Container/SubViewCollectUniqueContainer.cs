using System;
using UnityEngine;

public class SubViewCollectUniqueContainer<TSubViewCollectLocator, TSubViewLocator, TViewLoader> : ISubViewCollectContainer
    where TSubViewCollectLocator: MonoBehaviour, ISubViewCollectLocator
    where TSubViewLocator: MonoBehaviour, ISubViewLocator
    where TViewLoader: IViewLoader
{
    private SubViewDisplay subViewDisplay;
    private IViewLoader viewLoader;

    private TSubViewCollectLocator subViewCollectLocator;

    public SubViewCollectUniqueContainer(int poolCapacity, int preDestroyCapacity = 10, int preDestroyMillisecondsDelay = 10)
    {
        viewLoader = (TViewLoader)Activator.CreateInstance
        (
            typeof(TViewLoader), poolCapacity, preDestroyCapacity, preDestroyMillisecondsDelay
        );
    }
    
    void ISubViewCollectContainer.AddSubViewContainerType(SubViewDisplay subViewDisplay) => this.subViewDisplay = subViewDisplay;
    
    ISubViewCollectLocator ISubViewCollectContainer.AddLocator(GameObject goLocator)
    {
        subViewCollectLocator = goLocator.AddComponent<TSubViewCollectLocator>();
        return subViewCollectLocator;
    }

    SubViewDisplay ISubViewCollectContainer.GetSubViewDisplay() => subViewDisplay;
    IViewLoader ISubViewCollectContainer.GetViewLoader() => viewLoader;
}
