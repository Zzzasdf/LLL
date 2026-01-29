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
            .AddWindowService(
                new Dictionary<ViewLayer, ILayerContainer>
                {
                    [ViewLayer.Bg] = new LayerUniqueContainer<LayerRaycastBlockingLocator, ViewRaycastBlockingLocator, ViewUniqueLoader>(poolCapacity: 1),
                    [ViewLayer.Permanent] = new LayerMultipleContainer<LayerUnitLocator, ViewUnitLocator, ViewUniqueLoader>(poolCapacity: 1),
                    [ViewLayer.FullScreen] = new LayerUniqueContainer<LayerRaycastBlockingLocator, ViewRaycastBlockingLocator, ViewUniqueLoader>(poolCapacity: 1),
                    [ViewLayer.Window] = new LayerUniqueContainer<LayerMaskBlackLocator, ViewMaskTransparentClickLocator, ViewUniqueLoader>(poolCapacity: 1),
                    [ViewLayer.Popup] = new LayerUniqueContainer<LayerMaskBlackLocator, ViewMaskBlackClickLocator, ViewUnitLoader>(poolCapacity: 1),
                    [ViewLayer.Tip] = new LayerMultipleContainer<LayerUnitLocator, ViewUnitLocator, ViewUniqueLoader>(poolCapacity: 1),
                    [ViewLayer.System] = new LayerUniqueContainer<LayerRaycastBlockingLocator, ViewRaycastBlockingLocator, ViewUnitLoader>(poolCapacity: 1),
                }, 
                new Dictionary<SubViewDisplay, ISubViewCollectContainer>
                {
                    [SubViewDisplay.Unique] = new SubViewCollectUniqueContainer<SubViewCollectUnitLocator, SubViewUnitLocator, ViewUniqueLoader>(poolCapacity: 1),
                    [SubViewDisplay.Multiple] = new SubViewCollectMultipleContainer<SubViewCollectUnitLocator, SubViewUnitLocator, ViewMultipleLoader>(poolCapacity: 1),
                },
                new Dictionary<ViewLayer, List<IViewConfigure>>
                {
                    [ViewLayer.Bg] = new List<IViewConfigure>
                    {
                    },
                    [ViewLayer.Permanent] = new List<IViewConfigure>
                    {
                        AddView<MainView, MainViewModel>(services)
                            .AddSubType(SubViewDisplay.Multiple, new Dictionary<SubViewType, IViewCheck>
                            {
                                [SubViewType.MiniMapView] = null,
                                [SubViewType.MiniChatView] = null,
                                [SubViewType.EntryButtonGroupView] = null,
                            }),
                    },
                    [ViewLayer.FullScreen] = new List<IViewConfigure>
                    {
                        AddView<StartView, StartViewModel>(services),
                        AddView<SelectRoleView, SelectRoleViewModel>(services),
                        AddView<CreateRoleView, CreateRoleViewModel>(services),
                        AddView<ActivityView, ActivityViewModel>(services)
                            .AddSubType(SubViewDisplay.Unique, new Dictionary<SubViewType, IViewCheck>
                            {
                                [SubViewType.SubActivity] = new SubActivityCheck(1),
                                [SubViewType.SubActivity2] = null,
                            }),
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
                }, 
                new List<ISubViewConfigure>
                {
                    AddSubView<MiniMapView, MiniMapViewModel>(services, new List<SubViewType>
                    {
                        SubViewType.MiniMapView,
                    }),
                    AddSubView<MiniChatView, MiniChatViewModel>(services, new List<SubViewType>
                    {
                        SubViewType.MiniChatView,
                    }),
                    AddSubView<EntryButtonGroupView, EntryButtonGroupViewModel>(services, new List<SubViewType>
                    {
                        SubViewType.EntryButtonGroupView,
                    }),
                    AddSubView<SubActivityView, SubActivityViewModel>(services, new List<SubViewType>
                    {
                        SubViewType.SubActivity,
                        SubViewType.SubActivity2,
                    }),
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
        ViewConfigure viewConfigure = new ViewConfigure(services);
        IViewConfigure configure = viewConfigure;
        configure.AddView<TView, TViewModel>();
        return viewConfigure;
    }
    private static SubViewConfigure AddSubView<TView, TViewModel>(IServiceCollection services, List<SubViewType> subViewTypes)
        where TView : IView 
        where TViewModel: class, IViewModel
    {
        SubViewConfigure subViewConfigure = new SubViewConfigure(services);
        ISubViewConfigure configure = subViewConfigure;
        configure.AddView<TView, TViewModel>(subViewTypes);
        return subViewConfigure;
    }
}
