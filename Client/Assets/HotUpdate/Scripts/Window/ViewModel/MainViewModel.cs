using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public partial class MainViewModel : ObservableRecipient,
    IViewModel
{
    [ObservableProperty]
    private string _modelLocation = "Cube";
    [ObservableProperty]
    private float _cameraDistance = 10;

    [RelayCommand]
    private void Close(IView view) => CloseAsync(view).Forget();
    private async UniTask CloseAsync(IView view)
    {
        await WeakReferenceMessenger.Default.SendViewHideAsync(view);
    }
}
