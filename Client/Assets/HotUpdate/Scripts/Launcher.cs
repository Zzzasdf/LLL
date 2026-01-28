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
            .AddWindowService(sp => new Dictionary<ViewLayer, ILayerContainer>
                {
                    [ViewLayer.Bg] = new LayerUniqueContainer<LayerRaycastBlockingLocator, ViewRaycastBlockingHelper, ViewUniqueLoader>(ViewLayer.Bg, poolCapacity: 1),
                    [ViewLayer.Permanent] = new LayerMultipleContainer<LayerUnitLocator, ViewUnitHelper, ViewUniqueLoader>(ViewLayer.Permanent, poolCapacity: 1),
                    [ViewLayer.FullScreen] = new LayerUniqueContainer<LayerRaycastBlockingLocator, ViewRaycastBlockingHelper, ViewUniqueLoader>(ViewLayer.FullScreen, poolCapacity: 1),
                    [ViewLayer.Window] = new LayerUniqueContainer<LayerMaskBlackLocator, ViewMaskTransparentClickHelper, ViewUniqueLoader>(ViewLayer.Window, poolCapacity: 1),
                    [ViewLayer.Popup] = new LayerUniqueContainer<LayerMaskBlackLocator, ViewMaskBlackClickHelper, ViewUnitLoader>(ViewLayer.Popup, poolCapacity: 1),
                    [ViewLayer.Tip] = new LayerMultipleContainer<LayerUnitLocator, ViewUnitHelper, ViewUniqueLoader>(ViewLayer.Tip, poolCapacity: 1),
                    [ViewLayer.System] = new LayerUniqueContainer<LayerRaycastBlockingLocator, ViewRaycastBlockingHelper, ViewUnitLoader>(ViewLayer.System, poolCapacity: 1),
                }, new Dictionary<ViewLayer, List<IViewConfigure>>
                {
                    [ViewLayer.Bg] = new List<IViewConfigure>
                    {
                    },
                    [ViewLayer.Permanent] = new List<IViewConfigure>
                    {
                        AddView<MainView, MainViewModel>(services)
                            .AddSubType<SubActivityView>(),
                    },
                    [ViewLayer.FullScreen] = new List<IViewConfigure>
                    {
                        AddView<StartView, StartViewModel>(services),
                        AddView<SelectRoleView, SelectRoleViewModel>(services),
                        AddView<CreateRoleView, CreateRoleViewModel>(services)
                    },
                    [ViewLayer.Window] = new List<IViewConfigure>
                    {
                    },
                    [ViewLayer.Popup] = new List<IViewConfigure>
                    {
                        AddView<SettingsView, SettingsViewModel>(services),
                        AddView<HelpView, HelpViewModel>(services),
                        AddView<ConfirmAgainView, ConfirmAgainViewModel>(services),
                    },
                    [ViewLayer.Tip] = new List<IViewConfigure>
                    {
                    },
                    [ViewLayer.System] = new List<IViewConfigure>
                    {
                        AddView<LoadingView, LoadingViewModel>(services),
                    },
                }, new List<(Func<ISubViewContainer>, List<ISubViewConfigure>)>
                {
                    // 同类型子界面只允许同时出现一个
                    (()=> new SubViewUniqueContainer(),
                        new List<ISubViewConfigure>
                        {
                            AddSubView<SubActivityView, SubActivityViewModel>(services)
                                .AddCheck(new SubActivityCheck(1)),
                        }
                    ),
                    // 同类型子界面允许出现多个
                    (()=> new SubViewMultipleContainer(),
                        new List<ISubViewConfigure>
                        {
                        }
                    )
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

        // 账号级别数据
        services.AddAccountLevelModel<AccountModel>();
        services.AddAccountLevelModel<GlobalSettingsModel>();
        
        // 角色级别数据
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
    
    private static ViewConfigure AddView<TView, TViewModel>(IServiceCollection services) 
        where TView : IView 
        where TViewModel: class, IViewModel
    {
        return new ViewConfigure(services).AddView<TView, TViewModel>();
    }
    private static SubViewConfigure AddSubView<TView, TViewModel>(IServiceCollection services)
        where TView : IView 
        where TViewModel: class, IViewModel
    {
        return new SubViewConfigure(services).AddView<TView, TViewModel>();
    }
}
