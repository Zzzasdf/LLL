using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public partial class SettingsViewModel : ObservableObject, IViewModel
{
    private readonly IDataService dataService;
    private readonly GlobalSettingsModel globalSettingsModel;
    
    public SettingsViewModel(IDataService dataService, GlobalSettingsModel globalSettingsModel)
    {
        
    }
    
    [RelayCommand]
    private void Close(ViewEntityBase view) => CloseAsync(view).Forget();
    private async UniTask CloseAsync(ViewEntityBase view)
    {
        await WeakReferenceMessenger.Default.SendViewHideAsync(view);
    }
}
