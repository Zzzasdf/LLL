using System;
using Microsoft.Extensions.DependencyInjection;

public static class ModelServiceCollectionServiceExtensions
{
    public static IServiceCollection AddAccountLevelModel<TService>(
        this IServiceCollection services)
        where TService : class, IAccountLevelModel, new()
    {
        return services.AddTransient<TService>(sp => sp.GetRequiredService<IDataService>().AccountLevelGet<TService>());
    }
    public static IServiceCollection AddAccountLevelModel<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory)
        where TService : class, IAccountLevelModel, new()
    {
        return services.AddTransient<TService>(implementationFactory);
    }
    
    public static IServiceCollection AddRoleLevelModel<TService>(
        this IServiceCollection services)
        where TService : class, new()
    {
        return services.AddTransient<TService>(sp => sp.GetRequiredService<IDataService>().Get<TService>());
    }
    
    public static IServiceCollection AddRoleLevelModel<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory)
        where TService : class, new()
    {
        return services.AddTransient<TService>(implementationFactory);
    }
}
