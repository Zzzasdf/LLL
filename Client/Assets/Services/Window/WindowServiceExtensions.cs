using System;
using Microsoft.Extensions.DependencyInjection;

public static class WindowServiceExtensions
{
    public static void AddWindowService(this IServiceCollection services)
    {
        services.AddSingleton<IWindowService, WindowService>(sp => new WindowService(
            sp.GetService<ILogService>()));
    }
}
