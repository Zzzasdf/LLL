using System;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    private void Awake()
    {
        var serviceProvider = ConfigureServices();
        _ = new ServiceContext(serviceProvider);
    }
    
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
    
        services.AddDeviceService();
        services.AddHardwareService();
        services.AddLoggerService();
        
        return services.BuildServiceProvider();
    }

    private void Start()
    {
        Hello.Run();
    }
}
