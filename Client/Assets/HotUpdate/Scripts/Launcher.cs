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

        services.AddWindowService(new LayerConfigures(new Dictionary<ViewLayer, ILayerConfigure>
            {
                [ViewLayer.Bg] = new LayerConfigure<LayerRaycastBlockingLocator, LayerUniqueContainer, ViewUniqueLoader, ViewRaycastBlockingLocator>(poolCapacity: 1),
                [ViewLayer.Permanent] = new LayerConfigure<LayerUnitLocator, LayerMultipleContainer, ViewUniqueLoader, ViewUnitLocator>(poolCapacity: 1),
                [ViewLayer.FullScreen] = new LayerConfigure<LayerRaycastBlockingLocator, LayerUniqueContainer, ViewUniqueLoader, ViewRaycastBlockingLocator>(poolCapacity: 1),
                [ViewLayer.Window] = new LayerConfigure<LayerMaskBlackLocator, LayerUniqueContainer, ViewUniqueLoader, ViewMaskTransparentClickLocator>(poolCapacity: 1),
                [ViewLayer.Popup] = new LayerConfigure<LayerMaskBlackLocator, LayerUniqueContainer, ViewUnitLoader, ViewMaskBlackClickLocator>(poolCapacity: 1),
                [ViewLayer.Tip] = new LayerConfigure<LayerUnitLocator, LayerMultipleContainer, ViewUniqueLoader, ViewUnitLocator>(poolCapacity: 1),
                [ViewLayer.System] = new LayerConfigure<LayerRaycastBlockingLocator, LayerUniqueContainer, ViewUnitLoader, ViewRaycastBlockingLocator>(poolCapacity: 1),
            }),new SubViewCollectConfigures(new Dictionary<SubViewCollect, ISubViewDisplayConfigure>
            {
                [SubViewCollect.Selector] = new SubViewCollectConfigure<SubViewCollectLocator, SubViewCollectContainer, SubViewsSelectorLocator, ViewUniqueLoader, SubViewUnitLocator>(poolCapacity: 1),
                [SubViewCollect.MultiOpener] = new SubViewCollectConfigure<SubViewCollectLocator, SubViewCollectContainer, SubViewsMultiOpenerLocator, ViewUniqueLoader, SubViewUnitLocator>(poolCapacity: 1),
            }), new ViewConfigures(new Dictionary<ViewLayer, List<IViewConfigure>>
            {
                [ViewLayer.Bg] = new List<IViewConfigure>
                {
                },
                [ViewLayer.Permanent] = new List<IViewConfigure>
                {
                    View<MainView, MainViewModel>(services)
                        .SubViews(SubViewCollect.MultiOpener, new List<ISubViewConfigure>
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
                        .SubViews(SubViewCollect.Selector, new List<ISubViewConfigure>
                        {
                            SubView<SubActivityView, SubActivityViewModel>(services, SubViewShow.SubActivity, new SubActivityCheck(1, "活动1")),
                            SubView<SubActivityView, SubActivityViewModel>(services, SubViewShow.SubActivity2, new EntryNameCheck("活动2")),
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
            }
        ));
        
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
}
