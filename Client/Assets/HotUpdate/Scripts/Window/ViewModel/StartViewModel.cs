using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

public partial class StartViewModel : ObservableObject
{
    private readonly IDataService dataService;
    private readonly StartModel startModel;
    private readonly IDeviceService deviceService;
    
    public StartViewModel(IDataService dataService, StartModel startModel,
        IDeviceService deviceService)
    {
        this.dataService = dataService;
        this.startModel = startModel;
        this.deviceService = deviceService;
    }

    [RelayCommand]
    private void Start()
    {
        WeakReferenceMessenger.Default.Send(new GameStateMessage
        {
            gameState = ProcedureService.GameState.Role,
        });
    }

    [RelayCommand]
    private void Settings()
    {
        Ioc.Default.ShowViewAsync<SettingsView>();
    }

    [RelayCommand]
    private void Help()
    {
        Ioc.Default.ShowViewAsync<HelpView>();
    }

    [RelayCommand]
    private void Quit()
    {
        deviceService.ApplicationQuit();
    }
}
