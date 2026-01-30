using System;
using Microsoft.Extensions.DependencyInjection;

public partial class Launcher
{
#region AddView
    private static ViewConfigure View<TView, TViewModel>(IServiceCollection services) 
        where TView : ViewBase<TViewModel>, IView 
        where TViewModel: class, IViewModel
    {
        Type type = typeof(TView);
        services.AddTransient<TViewModel>();
        return new ViewConfigure(type);
    }
    private static ViewConfigure View<TView, TViewModel>(IServiceCollection services, IViewCheck viewCheck) 
        where TView : ViewBase<TViewModel>, IView 
        where TViewModel: class, IViewModel
    {
        Type type = typeof(TView);
        services.AddTransient<TViewModel>();
        return new ViewConfigure(type, viewCheck);
    }
#endregion

#region AddSubView
    private static SubViewConfigure SubView<TView, TViewModel>(IServiceCollection services) 
        where TView : ViewBase<TViewModel>, IView 
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
