using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    private IServiceProvider serviceProvider;
    private void Awake()
    {
        serviceProvider = ConfigureServices();
        Ioc.Default.ConfigureServices(serviceProvider);
    }
    
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
    
        services.AddDataService();
        services.AddDeviceService();
        services.AddHardwareService();
        services.AddLogService();
        
        services
            .AddTransient<ProcedurePreload>()
            .AddTransient<ProcedureStart>()
            .AddTransient<ProcedureRole>()
            .AddTransient<ProcedureMain>()
            .AddTransient<ProcedureBattle>()
            .AddSingleton<ProcedureService>(sp => new ProcedureService(
            new Dictionary<ProcedureService.GameState, IProcedure>
            {
                [ProcedureService.GameState.Preload] = sp.GetService<ProcedurePreload>(),
                [ProcedureService.GameState.Start] = sp.GetService<ProcedureStart>(),
                [ProcedureService.GameState.Role] = sp.GetService<ProcedureRole>(),
                [ProcedureService.GameState.Main] = sp.GetService<ProcedureMain>(),
                [ProcedureService.GameState.Battle] = sp.GetService<ProcedureBattle>(),
            }));

        services
            .AddTransient<StackContainer>(sp => new StackContainer(sp.GetService<ILogService>()))
            .AddTransient<QueueContainer>(sp => new QueueContainer(sp.GetService<ILogService>()))
            .AddTransient<PopupContainer>(sp => new PopupContainer(sp.GetService<ILogService>()))
            .AddSingleton<IViewService, ViewService>(sp => new ViewService(
                new Dictionary<ViewLayer, ILayerContainer>
                {
                    [ViewLayer.Bg] = sp.GetService<StackContainer>().BindParam(ViewLayer.Bg, 8),
                    [ViewLayer.Permanent] = sp.GetService<PopupContainer>().BindParam(ViewLayer.Permanent, -1),
                    [ViewLayer.FullScreen] = sp.GetService<StackContainer>().BindParam(ViewLayer.FullScreen, 8),
                    [ViewLayer.Window] = sp.GetService<StackContainer>().BindParam(ViewLayer.Window, 8),
                    [ViewLayer.Popup] = sp.GetService<PopupContainer>().BindParam(ViewLayer.Popup, 8),
                    [ViewLayer.Tip] = sp.GetService<PopupContainer>().BindParam(ViewLayer.Tip, 8),
                    [ViewLayer.System] = sp.GetService<QueueContainer>().BindParam(ViewLayer.System, 1),
                }, 
                sp.GetService<ILogService>()
            ));
             
        services.AddView<StartView>(ViewLayer.FullScreen).AddSingleton<StartViewModel>(
            sp => new StartViewModel(
                sp.GetService<IDataService>(), 
                sp.GetService<IDataService>().Get<StartModel>(),
                sp.GetService<IDeviceService>()));
        services.AddView<SettingsView>(ViewLayer.FullScreen).AddSingleton<SettingsViewModel>(
            sp => new SettingsViewModel(
                sp.GetService<IDataService>(), 
                sp.GetService<IDataService>().Get<SettingsModel>()));
        services.AddView<RoleView>(ViewLayer.FullScreen).AddSingleton<RoleViewModel>(
            sp => new RoleViewModel(
                sp.GetService<IDataService>(), 
                sp.GetService<IDataService>().Get<RoleModel>()));
        
        services.AddView<HelpView>(ViewLayer.Window).AddSingleton<HelpViewModel>(
            sp => new HelpViewModel(
                sp.GetService<IDataService>(), 
                sp.GetService<IDataService>().Get<HelpModel>()));
        
        services.AddView<LoadingView>(ViewLayer.System).AddSingleton<LoadingViewModel>(
            sp => new LoadingViewModel(
                sp.GetService<IDataService>(), 
                sp.GetService<IDataService>().Get<LoadingModel>()));
        
        return services.BuildServiceProvider();
    }

    private void Start()
    {
        // 创建 UICanvas
        new GameObject("UICanvas").AddComponent<UICanvasLocator>().Build(serviceProvider.GetService<IViewService>());
        
        // 进入预载流程
        serviceProvider.GetService<ProcedureService>();
        WeakReferenceMessenger.Default.Send(new GameStateMessage
        {
            gameState = ProcedureService.GameState.Preload,
        });
    }
}
