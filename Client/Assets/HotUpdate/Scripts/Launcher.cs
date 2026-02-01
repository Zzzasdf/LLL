using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public partial class Launcher : MonoBehaviour
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

        services.AddEntityPoolService();
        services.AddSingleton<ViewPool>(sp => EntityPool<ViewPool>(sp, EntityPoolType.View, poolCapacity: 5, preDestroyCapacity: 10, preDestroyMillisecondsDelay: 10));
        services.AddTransient<ViewUnitLoader>(sp => new ViewUnitLoader(sp.GetRequiredService<ViewPool>()));
        services.AddTransient<ViewUniqueLoader>(sp => new ViewUniqueLoader(sp.GetRequiredService<ViewPool>()));
        services.AddTransient<ViewMultipleLoader>(sp => new ViewMultipleLoader(sp.GetRequiredService<ViewPool>()));
        services.AddSingleton<SubViewPool>(sp => EntityPool<SubViewPool>(sp, EntityPoolType.SubView, poolCapacity: 5, preDestroyCapacity: 10, preDestroyMillisecondsDelay: 10));
        services.AddTransient<SubViewUnitLoader>(sp => new SubViewUnitLoader(sp.GetRequiredService<SubViewPool>()));
        services.AddTransient<SubViewUniqueLoader>(sp => new SubViewUniqueLoader(sp.GetRequiredService<SubViewPool>()));
        services.AddTransient<SubViewMultipleLoader>(sp => new SubViewMultipleLoader(sp.GetRequiredService<SubViewPool>()));
        
        services.AddTransient<ViewLayerUniqueContainer>();
        services.AddTransient<ViewLayerMultipleContainer>();
        services.AddTransient<SubViewLayerContainer>();
        
        services.AddWindowService(
            new Dictionary<ViewLayer, Type>
            {
                [ViewLayer.Bg] = typeof(ViewLayerBgLocator),
                [ViewLayer.Permanent] = typeof(ViewLayerPermanentLocator),
                [ViewLayer.FullScreen] = typeof(ViewLayerFullScreenLocator),
                [ViewLayer.Window] = typeof(ViewLayerWindowLocator),
                [ViewLayer.Popup] = typeof(ViewLayerPopupLocator),
                [ViewLayer.Tip] = typeof(ViewLayerTipLocator),
                [ViewLayer.System] = typeof(ViewLayerSystemLocator),
            },
            new Dictionary<ViewLayer, List<IViewConfigure>>
            {
                [ViewLayer.Bg] = new List<IViewConfigure>
                {
                },
                [ViewLayer.Permanent] = new List<IViewConfigure>
                {
                    View<MainView, MainViewModel>(services)
                        .SubLayer<SubViewLayerMultiLocator>(new List<ISubViewConfigure>
                        {
                            SubView<MiniMapView, MiniMapViewModel>(services, SubViewShow.MiniMapView),
                            SubView<MiniChatView, MiniChatViewModel>(services, SubViewShow.MiniChatView),
                            SubView<EntryButtonGroupView, EntryButtonGroupViewModel>(services, SubViewShow.EntryButtonGroupView),
                        }),
                },
                [ViewLayer.FullScreen] = new List<IViewConfigure>
                {
                    View<StartView, StartViewModel>(services),
                    View<SelectRoleView, SelectRoleViewModel>(services),
                    View<CreateRoleView, CreateRoleViewModel>(services),
                    View<ActivityView, ActivityViewModel>(services)
                        .SubLayer<SubViewLayerSelectLocator>(new List<ISubViewConfigure>
                        {
                            SubView<SubActivityView, SubActivityViewModel>(services, SubViewShow.SubActivity, new SubActivityCheck(1, "Activity 1")),
                            SubView<SubActivityView, SubActivityViewModel>(services, SubViewShow.SubActivity2, new EntryNameCheck("Activity 2")),
                        }),
                },
                [ViewLayer.Window] = new List<IViewConfigure>
                {
                },
                [ViewLayer.Popup] = new List<IViewConfigure>
                {
                    View<SettingsView, SettingsViewModel>(services),
                    View<HelpView, HelpViewModel>(services),
                    View<ConfirmAgainView, ConfirmAgainViewModel>(services),
                },
                [ViewLayer.Tip] = new List<IViewConfigure>
                {
                },
                [ViewLayer.System] = new List<IViewConfigure>
                {
                    View<LoadingView, LoadingViewModel>(services),
                },
            });
        
        
        services
            .AddTransient<ProcedurePreload>()
            .AddTransient<ProcedureStart>()
            .AddTransient<ProcedureSelectRole>()
            .AddTransient<ProcedureCreateRole>()
            .AddTransient<ProcedureInit>()
            .AddTransient<ProcedureMain>()
            .AddTransient<ProcedureBattle>()
            .AddSingleton(sp => new ProcedureService(
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
        services.AddRoleLevelModel(sp => sp.GetRequiredService<IDataService>().Get<RoleModel>()
            .Bind(sp.GetRequiredService<IDataService>().AccountLevelGet<AccountModel>().GetSelectedAccountRoleSimpleModel()));

        return services.BuildServiceProvider();
    }

    private void Start()
    {
        // 创建 UI 层级
        serviceProvider.GetRequiredService<IViewService>();
        
        // 初始化服务（事件订阅）
        serviceProvider.GetRequiredService<ProcedureService>();

        // 进入预载流程
        WeakReferenceMessenger.Default.SendProcedureSwap(ProcedureService.GameState.Preload);
    }
}
