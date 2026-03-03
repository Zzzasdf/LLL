using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public partial class Launcher
{
#region AddPool
    private static Type ReusableDisposable<TReusable>()
        where TReusable: class, IReusableDisposable, new()
    {
        return typeof(TReusable);
    }
#endregion
    
#region AddEntityPool
    private static TEntityPool EntityPool<TEntityPool>(IServiceProvider sp, EntityPoolType entityPoolType,
        int poolCapacity, int preDestroyCapacity, int preDestroyMillisecondsDelay) 
        where TEntityPool : MonoBehaviour, IEntityPool
    {
        IEntityPoolService entityPoolService = sp.GetRequiredService<IEntityPoolService>();
        Transform parent = entityPoolService.Transform();
        GameObject goPool = new GameObject(entityPoolType.ToString());
        goPool.transform.SetParent(parent);
        IEntityPool entityPool = goPool.AddComponent<TEntityPool>();
        entityPool.Init(poolCapacity, preDestroyCapacity, preDestroyMillisecondsDelay);
        entityPoolService.Add(entityPoolType, entityPool);
        return (TEntityPool)entityPool;
    }
#endregion
    
#region AddView
    #region Branch
    private static ViewConfigure View<TView, TViewModel, TViewDriver, TViewLayerCore, TViewLayerDriver>(IServiceCollection services, ViewType viewType, List<IViewConfigure> subViewConfigures) 
        where TView : ViewEntityBase<TViewModel>, IView 
        where TViewModel: class, IViewModel
        where TViewDriver: MonoBehaviour, IViewDriver
        where TViewLayerCore: class, IViewLayerCore
        where TViewLayerDriver: MonoBehaviour, IViewLayerDriver
    {
        return View<TView, TViewModel, ViewLoader, TViewDriver, TViewLayerCore, TViewLayerDriver>(services, viewType, null, subViewConfigures);
    }
    private static ViewConfigure View<TView, TViewModel, TViewLoader, TViewDriver, TViewLayerCore, TViewLayerDriver>(IServiceCollection services, ViewType viewType, List<IViewConfigure> subViewConfigures) 
        where TView : ViewEntityBase<TViewModel>, IView 
        where TViewModel: class, IViewModel
        where TViewLoader: class, IViewLoader
        where TViewDriver: MonoBehaviour, IViewDriver
        where TViewLayerCore: class, IViewLayerCore
        where TViewLayerDriver: MonoBehaviour, IViewLayerDriver
    {
        return View<TView, TViewModel, TViewLoader, TViewDriver, TViewLayerCore, TViewLayerDriver>(services, viewType, null, subViewConfigures);
    }
    private static ViewConfigure View<TView, TViewModel, TViewDriver, TViewLayerCore, TViewLayerDriver>(IServiceCollection services, ViewType viewType, IViewCheck viewCheck, List<IViewConfigure> subViewConfigures)
        where TView : ViewEntityBase<TViewModel>, IView
        where TViewModel : class, IViewModel
        where TViewDriver : MonoBehaviour, IViewDriver
        where TViewLayerCore : class, IViewLayerCore
        where TViewLayerDriver : MonoBehaviour, IViewLayerDriver
    {
        return View<TView, TViewModel, ViewLoader, TViewDriver, TViewLayerCore, TViewLayerDriver>(services, viewType, viewCheck, subViewConfigures);
    }
    private static ViewConfigure View<TView, TViewModel, TViewLoader, TViewDriver, TViewLayerCore, TViewLayerDriver>(IServiceCollection services, ViewType viewType, IViewCheck viewCheck, List<IViewConfigure> subViewConfigures) 
        where TView : ViewEntityBase<TViewModel>, IView 
        where TViewModel: class, IViewModel
        where TViewLoader: class, IViewLoader
        where TViewDriver: MonoBehaviour, IViewDriver
        where TViewLayerCore: class, IViewLayerCore
        where TViewLayerDriver: MonoBehaviour, IViewLayerDriver
    {
        Type type = typeof(TView);
        services.AddTransient<TViewModel>();
        Type viewLoaderType = typeof(TViewLoader);
        Type viewDriverType = typeof(TViewDriver);
        Type viewLayerCoreType = typeof(TViewLayerCore);
        Type viewLayerDriverType = typeof(TViewLayerDriver);
        return new ViewConfigure(type, viewType, viewCheck, viewLoaderType, viewDriverType, viewLayerCoreType, viewLayerDriverType, subViewConfigures);
    }
    #endregion

    #region Leaf
    private static ViewConfigure View<TView, TViewModel, TViewDriver>(IServiceCollection services, ViewType viewType, IViewCheck viewCheck = null) 
        where TView : ViewEntityBase<TViewModel>, IView 
        where TViewModel: class, IViewModel
        where TViewDriver: MonoBehaviour, IViewDriver
    {
        return View<TView, TViewModel, ViewLoader, TViewDriver>(services, viewType, viewCheck);
    }
    private static ViewConfigure View<TView, TViewModel, TViewLoader, TViewDriver>(IServiceCollection services, ViewType viewType, IViewCheck viewCheck = null) 
        where TView : ViewEntityBase<TViewModel>, IView 
        where TViewModel: class, IViewModel
        where TViewLoader: class, IViewLoader
        where TViewDriver: MonoBehaviour, IViewDriver
    {
        Type type = typeof(TView);
        services.AddTransient<TViewModel>();
        Type viewLoaderType = typeof(TViewLoader);
        Type viewDriverType = typeof(TViewDriver);
        return new ViewConfigure(type, viewType, viewCheck, viewLoaderType, viewDriverType, null, null, null);
    }
    #endregion
#endregion
}
