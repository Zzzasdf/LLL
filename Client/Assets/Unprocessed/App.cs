using System;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public class App : MonoBehaviour
{
    // private IServiceProvider Services { get; set; }
    //
    // private void Awake()
    // {
    //     Services = ConfigureServices();
    // }

    // private IServiceProvider ConfigureServices()
    // {
    //     var services = new ServiceCollection();
    //
    //     services.AddSingleton<IMyLogger, MyLogger>();
    //     services.AddSingleton<DeviceService>();
    //     services.AddSingleton<HardwareService>();
    //
    //     services.AddSingleton(sp => new MainWindow { DataContext = sp.GetRequiredService<MainViewModel>() });
    //     services.AddSingleton<MainViewModel>();
    //     
    //     services.AddSingleton<ISystemTime, SystemTime>();
    //
    //     return services.BuildServiceProvider();
    // }
    //
    // private void Start()
    // {
    //     var mainWindow = Services.GetService<MainWindow>();
    //     mainWindow!.Show();
    // }
}
