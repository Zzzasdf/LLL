using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public static class ViewServiceProviderServiceExtensions
{
    private static Dictionary<Type, ViewLayer> windowLayers = new Dictionary<Type, ViewLayer>();
    
    public static IServiceCollection AddView<TView>(this IServiceCollection services, ViewLayer viewLayer)
        where TView : IView
    {
        windowLayers.Add(typeof(TView), viewLayer);
        return services;
    }
    
    public static UniTask<T> ShowViewAsync<T>(this IServiceProvider provider) where T: class, IView
    {
        var windowService = provider.GetService<IViewService>();
        Type type = typeof(T);
        if (!windowLayers.TryGetValue(type, out ViewLayer windowLayer))
        {
            var logService = provider.GetService<ILogService>();
            logService.Error($"未定义 Window: {type.Name} 的 {nameof(ViewLayer)}");
            return new UniTask<T>(default);;
        }
        return windowService.ShowAsync<T>(windowLayer);
    }
}
