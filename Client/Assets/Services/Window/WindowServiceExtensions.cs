using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public static class WindowServiceExtensions
{
    public static void AddWindowService(this IServiceCollection services, IViewConfigure viewConfigure)
    {
        services.AddSingleton<IViewService, ViewService>(sp =>
        {
            IViewService viewService = new GameObject(nameof(ViewService)).AddComponent<ViewService>();
            viewService.Bind(viewConfigure);
            return (ViewService)viewService;
        });
    }
}
