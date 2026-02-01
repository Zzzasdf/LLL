using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public static class EntityPoolServiceExtensions
{
    public static void AddEntityPoolService(this IServiceCollection services)
    {
        services.AddSingleton<IEntityPoolService, EntityPoolService>(sp => 
            new GameObject(nameof(EntityPoolService)).AddComponent<EntityPoolService>());
    }
}
