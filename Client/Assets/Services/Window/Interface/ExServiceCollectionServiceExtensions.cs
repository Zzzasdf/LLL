using System;
using Microsoft.Extensions.DependencyInjection;

public static class ExServiceCollectionServiceExtensions
{
    public static IServiceCollection AddSubView<TView, TViewModel>(this IServiceCollection services)
        where TView : IView 
        where TViewModel: class, IViewModel
    {
        return services.AddTransient<TViewModel>();
    }
    
    public static IServiceCollection AddAccountLevelModel<TAccountLevelModel>(
        this IServiceCollection services)
        where TAccountLevelModel : class, IAccountLevelModel, new()
    {
        return services.AddTransient<TAccountLevelModel>(sp => sp.GetRequiredService<IDataService>().AccountLevelGet<TAccountLevelModel>());
    }
    public static IServiceCollection AddAccountLevelModel<TAccountLevelModel>(
        this IServiceCollection services,
        Func<IServiceProvider, TAccountLevelModel> implementationFactory)
        where TAccountLevelModel : class, IAccountLevelModel, new()
    {
        return services.AddTransient<TAccountLevelModel>(implementationFactory);
    }
    
    public static IServiceCollection AddRoleLevelModel<TRoleLevelModel>(
        this IServiceCollection services)
        where TRoleLevelModel : class, new()
    {
        return services.AddTransient<TRoleLevelModel>(sp => sp.GetRequiredService<IDataService>().Get<TRoleLevelModel>());
    }
    public static IServiceCollection AddRoleLevelModel<TRoleLevelModel>(
        this IServiceCollection services,
        Func<IServiceProvider, TRoleLevelModel> implementationFactory)
        where TRoleLevelModel : class, new()
    {
        return services.AddTransient<TRoleLevelModel>(implementationFactory);
    }
}
