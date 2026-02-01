using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public static class WindowServiceExtensions
{
    public static void AddWindowService(this IServiceCollection services, 
        Dictionary<ViewLayer, Type> layerLocators,
        Dictionary<ViewLayer, List<IViewConfigure>> configures)
    {
        services.AddSingleton<IViewService, ViewService>(sp =>
        {
            IViewService viewService = new GameObject(nameof(ViewService)).AddComponent<ViewService>();
            viewService.Bind(layerLocators, configures);
            return (ViewService)viewService;
        });
    }
}
