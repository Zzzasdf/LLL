using Microsoft.Extensions.DependencyInjection;

public static class WindowServiceExtensions
{
    public static void AddWindowService(this IServiceCollection services, 
        ILayerConfigures layerConfigures, 
        ISubViewCollectConfigures subViewCollectConfigures,
        IViewConfigures viewConfigures)
    {
        services.AddSingleton<IViewService, ViewService>(sp => 
            new ViewService(layerConfigures, subViewCollectConfigures, viewConfigures));
    }
}
