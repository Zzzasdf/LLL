using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using UnityEngine;

public abstract class SubViewLayerLocatorBase<TSubViewLayerContainer, TSubViewLoader, TSubViewLocator>: MonoBehaviour, ISubViewLayerLocator
    where TSubViewLayerContainer: class, ISubViewLayerContainer
    where TSubViewLoader: class, IEntityLoader<Type, ViewEntityBase>, ISubViewLoader
    where TSubViewLocator: MonoBehaviour, ISubViewLocator
{
    protected IViewConfigure viewConfigure;
    protected SubViewShow? firstSubViewShow;
    protected TSubViewLayerContainer subViewsContainer;
    protected IEntityLoader<Type, ViewEntityBase> subViewLoader;

    public virtual void Init(IViewConfigure viewConfigure, SubViewShow? firstSubViewShow)
    {
        this.viewConfigure = viewConfigure;
        this.firstSubViewShow = firstSubViewShow;
        this.subViewsContainer = Ioc.Default.GetRequiredService<TSubViewLayerContainer>();
        this.subViewsContainer.Bind(this);
        this.subViewLoader = Ioc.Default.GetRequiredService<TSubViewLoader>();
    }

    public abstract void HideViews();
}
