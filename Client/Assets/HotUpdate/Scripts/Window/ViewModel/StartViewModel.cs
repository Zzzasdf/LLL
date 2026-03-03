using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public partial class StartViewModel : ObservableObject, IViewModel
{
    private readonly IDeviceService deviceService;
    
    public StartViewModel(IDeviceService deviceService)
    {
        this.deviceService = deviceService;
    }

    [RelayCommand]
    private void Start()
    {
        WeakReferenceMessenger.Default.SendProcedureSwap(ProcedureService.GameState.SelectRole);
    }

    [RelayCommand]
    private void ShaderExamples()
    {
    }

    [RelayCommand]
    private async Task Settings()
    {
        await WeakReferenceMessenger.Default.SendViewShowAsync(ViewType.SettingsView);
    }

    [RelayCommand]
    private async Task Help()
    {
        await WeakReferenceMessenger.Default.SendViewShowAsync(ViewType.HelpView);
    }

    [RelayCommand]
    private void Quit()
    {
        deviceService.ApplicationQuit();
    }
    
    [RelayCommand]
    private void Close(ViewEntityBase view) => CloseAsync(view).Forget();
    private async UniTask CloseAsync(ViewEntityBase view)
    {
        await WeakReferenceMessenger.Default.SendViewHideAsync(view);
    }
}
