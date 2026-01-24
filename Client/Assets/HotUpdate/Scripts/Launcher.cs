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

        services
            .AddTransient<StackContainer>()
            .AddTransient<QueueContainer>()
            .AddTransient<PopupContainer>()
            .AddWindowService(sp => new Dictionary<ViewLayer, ILayerContainer>
                {
                    [ViewLayer.Bg] = new StackContainer(warmCapacity: 8),
                    [ViewLayer.Permanent] = new PopupContainer(warmCapacity: -1),
                    [ViewLayer.FullScreen] = new StackContainer(warmCapacity: 8),
                    [ViewLayer.Window] = new StackContainer(warmCapacity: 8),
                    [ViewLayer.Popup] = new PopupContainer(warmCapacity: 8),
                    [ViewLayer.Tip] = new PopupContainer(warmCapacity: 8),
                    [ViewLayer.System] = new QueueContainer(capacity: 1),
                
                }, new Dictionary<ViewLayer, List<Type>>
                {
                    [ViewLayer.Bg] = new List<Type> { },
                    [ViewLayer.Permanent] = new List<Type>
                    {
                        AddView<MainView, MainViewModel>(services),
                    },
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
                }
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

        services.AddRoleLevelModel<RoleModel>(sp => sp.GetRequiredService<IDataService>().Get<RoleModel>()
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
        services.AddRoleLevelModel<TModel>();
        return typeof(TView);
    }
    private static Type AddViewWithAccount<TView, TViewModel, TModel>(IServiceCollection services) 
        where TView : IView 
        where TViewModel: class, IViewModel
        where TModel: class, IAccountLevelModel, new()
    {
        services.AddTransient<TViewModel>();
        services.AddAccountLevelModel<TModel>();
        return typeof(TView);
    }
}
