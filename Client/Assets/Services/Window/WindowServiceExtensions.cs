using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

public static class WindowServiceExtensions
{
    public static void AddWindowService(this IServiceCollection services, 
        Dictionary<ViewLayer, ILayerContainer> layerContainers, 
        Dictionary<SubViewDisplay, ISubViewCollectContainer> subViewContainers, 
        Dictionary<ViewLayer, List<IViewConfigure>> views, 
        List<ISubViewConfigure> subViews)
    {
        services.AddSingleton<IViewService, ViewService>(sp => new ViewService
            (layerContainers, views, subViewContainers, subViews));
    }
}
