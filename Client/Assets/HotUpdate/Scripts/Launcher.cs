using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;
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

        services.AddPoolService(new Dictionary<IFactoryObjectPool, List<Type>>
        {
            [new SharedFactoryObjectPool(10)] = new List<Type>
            {
                ReusableDisposable<ViewAutoExpander>(),
                // ReusableDisposable<ViewSingleResponder>(),
                // ReusableDisposable<ViewMultiResponder>(),
                // ReusableDisposable<ViewSelector>(),
            }
        });
        services.AddEntityPoolService();
        {
            services.AddSingleton<ViewLayerDriverPool>(sp => EntityPool<ViewLayerDriverPool>(sp, EntityPoolType.ViewLayerDriver, poolCapacity: 5, preDestroyCapacity: 10, preDestroyMillisecondsDelay: 10));
            {
                services.AddSingleton<ViewLayerDriverLoader>(sp => new ViewLayerDriverLoader(sp.GetRequiredService<ViewLayerDriverPool>()));
            }
            services.AddSingleton<ViewDriverPool>(sp => EntityPool<ViewDriverPool>(sp, EntityPoolType.ViewDriver, poolCapacity: 5, preDestroyCapacity: 10, preDestroyMillisecondsDelay: 10));
            {
                services.AddSingleton<ViewDriverLoader>(sp => new ViewDriverLoader(sp.GetRequiredService<ViewDriverPool>()));
            }
            services.AddSingleton<ViewPool>(sp => EntityPool<ViewPool>(sp, EntityPoolType.View, poolCapacity: 5, preDestroyCapacity: 10, preDestroyMillisecondsDelay: 10));
            {
                services.AddSingleton<ViewLoader>(sp => new ViewLoader(sp.GetRequiredService<ViewPool>()));
            }
        }

        services.AddWindowService(
            View<ServiceView, ServiceViewModel, ViewDriver_RaycastBlocking, ViewAutoExpander, ViewLayerDriver>(services, ViewType.Service, new List<IViewConfigure>
            {
                View<BgView, BgViewModel, ViewDriver, ViewAutoExpander, ViewLayerDriver>(services, ViewType.Bg, new List<IViewConfigure>
                {
                }),
                View<PermanentView, PermanentViewModel, ViewDriver, ViewAutoExpander, ViewLayerDriver_RaycastBlocking>(services, ViewType.Permanent, new List<IViewConfigure>
                {
                    View<LoginView, LoginViewModel, ViewDriver>(services, ViewType.LoadingView),
                    // View<MainView, MainViewModel, ViewDriver, ViewAutoExpander, ViewLayerDriver>(services, ViewType.MainView, new List<IViewConfigure>
                    // {
                    //     View<MiniMapView, MiniMapViewModel, ViewDriver>(services, ViewType.Main_MiniMapView),
                    //     View<MiniChatView, MiniChatViewModel, ViewDriver>(services, ViewType.Main_MiniChatView),
                    //     View<EntryButtonGroupView, EntryButtonGroupViewModel, ViewDriver>(services, ViewType.Main_EntryButtonGroupView),
                    // }),
                }),
                // View<BgView, BgViewModel, ViewLayerSingleResponder_RaycastBlocking>(services, ViewType.Bg, new List<IViewConfigure>
                // {
                // }),
                // View<PermanentView, PermanentViewModel, ViewLayerMultiResponder>(services, ViewType.Permanent, new List<IViewConfigure>
                // {
                //     View<MainView, MainViewModel, ViewLayerAutoExpander>(services, ViewType.MainView, new List<IViewConfigure>
                //     {
                //         View<MiniMapView, MiniMapViewModel, >(services, ViewType.Main_MiniMapView),
                //         View<MiniChatView, MiniChatViewModel>(services, ViewType.Main_MiniChatView),
                //         View<EntryButtonGroupView, EntryButtonGroupViewModel>(services, ViewType.Main_EntryButtonGroupView),
                //     }),
                // }),
                // View<FullScreenView, FullScreenViewModel, ViewLayerSingleResponder_RaycastBlocking>(services, ViewType.FullScreen, new List<IViewConfigure>
                // {
                //     View<StartView, StartViewModel, >(services, ViewType.StartView),
                //     View<SelectRoleView, SelectRoleViewModel, >(services, ViewType.SelectRoleView),
                //     View<CreateRoleView, CreateRoleViewModel, >(services, ViewType.CreateRoleView),
                //     View<ActivityView, ActivityViewModel, ViewLayerSelector>(services, ViewType.ActivityView, new List<IViewConfigure>
                //     {
                //         View<SubActivityView, SubActivityViewModel, >(services, ViewType.Activity_SubActivityView, new SubActivityCheck(1, "Activity 1")),
                //         View<SubActivityView, SubActivityViewModel, >(services, ViewType.Activity_SubActivity2View, new EntryNameCheck("Activity 2")),
                //     }),
                // }),
                // View<WindowView, WindowViewModel, ViewLayerSingleResponder_MaskBlack>(services, ViewType.Window, new List<IViewConfigure>
                // {
                // }),
                // View<PopupView, PopupViewModel, ViewLayerMultiResponder_MaskBlack>(services, ViewType.Popup, new List<IViewConfigure>
                // {
                //     View<SettingsView, SettingsViewModel, >(services, ViewType.SettingsView),
                //     View<HelpView, HelpViewModel, >(services, ViewType.HelpView),
                //     View<ConfirmAgainView, ConfirmAgainViewModel, >(services, ViewType.ConfirmAgainView),
                // }),
                // View<TipView, TipViewModel, ViewLayerMultiResponder>(services, ViewType.Tip, new List<IViewConfigure>
                // {
                // }),
                // View<SystemView, SystemViewModel, ViewLayerSingleResponder_RaycastBlocking>(services, ViewType.System, new List<IViewConfigure>
                // {
                //     View<LoadingView, LoadingViewModel, >(services, ViewType.System),
                // }),
            }));
        
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

    private void Start() => StartAsync().Forget();
    private async UniTask StartAsync()
    {
        // 创建 UI 层级
        serviceProvider.GetRequiredService<IViewService>();
        await WeakReferenceMessenger.Default.SendViewShowAsync(ViewType.Service);

        // 初始化服务（事件订阅）
        serviceProvider.GetRequiredService<ProcedureService>();

        // 进入预载流程
        // WeakReferenceMessenger.Default.SendProcedureSwap(ProcedureService.GameState.Preload);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            WeakReferenceMessenger.Default.SendViewShowAsync(ViewType.MainView).Forget();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            WeakReferenceMessenger.Default.SendViewShowAsync(ViewType.Main_MiniChatView).Forget();
        }
    }
}