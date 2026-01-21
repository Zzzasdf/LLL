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
            .AddTransient<StackContainer>()
            .AddTransient<QueueContainer>()
            .AddTransient<PopupContainer>()
            .AddWindowService(sp => new Dictionary<ViewLayer, ILayerContainer>
                {
                    [ViewLayer.Bg] = sp.GetRequiredService<StackContainer>().BindParam(ViewLayer.Bg, 8),
                    [ViewLayer.Permanent] = sp.GetRequiredService<PopupContainer>().BindParam(ViewLayer.Permanent, -1),
                    [ViewLayer.FullScreen] = sp.GetRequiredService<StackContainer>().BindParam(ViewLayer.FullScreen, 8),
                    [ViewLayer.Window] = sp.GetRequiredService<StackContainer>().BindParam(ViewLayer.Window, 8),
                    [ViewLayer.Popup] = sp.GetRequiredService<PopupContainer>().BindParam(ViewLayer.Popup, 8),
                    [ViewLayer.Tip] = sp.GetRequiredService<PopupContainer>().BindParam(ViewLayer.Tip, 8),
                    [ViewLayer.System] = sp.GetRequiredService<QueueContainer>().BindParam(ViewLayer.System, 1),
                
                }, new Dictionary<ViewLayer, List<Type>>
                {
                    [ViewLayer.Bg] = new List<Type> { },
                    [ViewLayer.Permanent] = new List<Type> { },
                    [ViewLayer.FullScreen] = new List<Type>
                    {
                        AddView<StartView, StartViewModel>(services),
                        AddViewWithAccount<SettingsView, SettingsViewModel, GlobalSettingsModel>(services),
                        AddViewWithAccount<SelectRoleView, SelectRoleViewModel, AccountModel>(services),
                        AddViewWithAccount<CreateRoleView, CreateRoleViewModel, AccountModel>(services),
                    },
                    [ViewLayer.Window] = new List<Type>
                    {
                        AddView<HelpView, HelpViewModel>(services),
                    },
                    [ViewLayer.Popup] = new List<Type> { },
                    [ViewLayer.Tip] = new List<Type> { },
                    [ViewLayer.System] = new List<Type>
                    {
                        AddView<LoadingView, LoadingViewModel>(services),
                    },
                }, 
                sp => sp.GetRequiredService<ILogService>()
            );
        
        services
            .AddTransient<ProcedurePreload>()
            .AddTransient<ProcedureStart>()
            .AddTransient<ProcedureSelectRole>()
            .AddTransient<ProcedureCreateRole>()
            .AddTransient<ProcedureInit>()
            .AddTransient<ProcedureMain>()
            .AddTransient<ProcedureBattle>()
            .AddSingleton<ProcedureService>(sp => new ProcedureService(
                new Dictionary<ProcedureService.GameState, IProcedure>
                {
                    [ProcedureService.GameState.Preload] = sp.GetRequiredService<ProcedurePreload>(),
                    [ProcedureService.GameState.Start] = sp.GetRequiredService<ProcedureStart>(),
                    [ProcedureService.GameState.SelectRole] = sp.GetRequiredService<ProcedureSelectRole>(),
                    [ProcedureService.GameState.CreateRole] = sp.GetRequiredService<ProcedureCreateRole>(),
                    [ProcedureService.GameState.Init] = sp.GetRequiredService<ProcedureInit>(),
                    [ProcedureService.GameState.Main] = sp.GetRequiredService<ProcedureMain>(),
                    [ProcedureService.GameState.Battle] = sp.GetRequiredService<ProcedureBattle>(),
                }));

        services.AddTransient<RoleModel>(sp => sp.GetRequiredService<IDataService>().Get<RoleModel>()
            .Bind(sp.GetRequiredService<IDataService>().AccountLevelGet<AccountModel>().GetSelectedAccountRoleSimpleModel()));
        
        return services.BuildServiceProvider();
    }

    private void Start()
    {
        // 初始化服务（事件订阅）
        serviceProvider.GetRequiredService<ProcedureService>();
        
        // 创建 UICanvas
        new GameObject("UICanvas").AddComponent<UICanvasLocator>().Build(serviceProvider.GetRequiredService<IViewService>());
        
        // 进入预载流程
        WeakReferenceMessenger.Default.SendProcedureSwap(ProcedureService.GameState.Preload);
    }
    
    private static Type AddView<TView, TViewModel>(IServiceCollection services) 
        where TView : IView 
        where TViewModel: class, IViewModel
    {
        services.AddTransient<TViewModel>();
        return typeof(TView);
    }
    private static Type AddView<TView, TViewModel, TModel>(IServiceCollection services) 
        where TView : IView 
        where TViewModel: class, IViewModel
        where TModel: class, new()
    {
        services.AddTransient<TViewModel>();
        services.AddSingleton<TModel>(sp => sp.GetRequiredService<IDataService>().Get<TModel>());
        return typeof(TView);
    }
    private static Type AddViewWithAccount<TView, TViewModel, TModel>(IServiceCollection services) 
        where TView : IView 
        where TViewModel: class, IViewModel
        where TModel: class, IAccountLevelModel, new()
    {
        services.AddTransient<TViewModel>();
        services.AddSingleton<TModel>(sp => sp.GetRequiredService<IDataService>().AccountLevelGet<TModel>());
        return typeof(TView);
    }
}
