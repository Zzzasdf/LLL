using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

public static class PoolServiceExtensions
{
    public static void AddPoolService(this IServiceCollection services, Dictionary<IFactoryObjectPool, List<Type>> customFactoryPools)
    {
        services.AddSingleton<IPoolService, PoolService>(sp => new PoolService(customFactoryPools));
    }
}
