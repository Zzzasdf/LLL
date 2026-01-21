using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

public static class WindowServiceExtensions
{
    public static void AddWindowService(this IServiceCollection services, 
        Func<IServiceProvider, Dictionary<ViewLayer, ILayerContainer>> layerContainersFunc, 
        Dictionary<ViewLayer, List<Type>> viewLayers, 
        Func<IServiceProvider, ILogService> logServiceFunc)
    {
        services.AddSingleton<IViewService, ViewService>(sp => new ViewService
            (layerContainersFunc(sp), viewLayers, logServiceFunc(sp)));
    }
}
