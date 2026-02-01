using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public partial class Launcher
{
#region AddPool
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
    private static ViewConfigure View<TView, TViewModel>(IServiceCollection services) 
        where TView : ViewEntityBase<TViewModel>, IView 
        where TViewModel: class, IViewModel
    {
        Type type = typeof(TView);
        services.AddTransient<TViewModel>();
        return new ViewConfigure(type);
    }
    private static ViewConfigure View<TView, TViewModel>(IServiceCollection services, IViewCheck viewCheck) 
        where TView : ViewEntityBase<TViewModel>, IView 
        where TViewModel: class, IViewModel
    {
        Type type = typeof(TView);
        services.AddTransient<TViewModel>();
        return new ViewConfigure(type, viewCheck);
    }
#endregion

#region AddSubView
    private static SubViewConfigure SubView<TView, TViewModel>(IServiceCollection services) 
        where TView : ViewEntityBase<TViewModel>, IView 
        where TViewModel: class, IViewModel
    {
        Type type = typeof(TView);
        services.AddTransient<TViewModel>();
        return new SubViewConfigure(type);
    }
    private static SubViewConfigure SubView<TView, TViewModel>(IServiceCollection services, SubViewShow subViewShow) 
        where TView : IView 
        where TViewModel: class, IViewModel
    {
        Type type = typeof(TView);
        services.AddTransient<TViewModel>();
        return new SubViewConfigure(type, subViewShow);
    }
    private static SubViewConfigure SubView<TView, TViewModel>(IServiceCollection services, IViewCheck viewCheck) 
        where TView : IView 
        where TViewModel: class, IViewModel
    {
        Type type = typeof(TView);
        services.AddTransient<TViewModel>();
        return new SubViewConfigure(type, viewCheck);
    }
    private static SubViewConfigure SubView<TView, TViewModel>(IServiceCollection services, SubViewShow subViewShow, IViewCheck viewCheck) 
        where TView : IView 
        where TViewModel: class, IViewModel
    {
        Type type = typeof(TView);
        services.AddTransient<TViewModel>();
        return new SubViewConfigure(type, subViewShow, viewCheck);
    }
#endregion
}
