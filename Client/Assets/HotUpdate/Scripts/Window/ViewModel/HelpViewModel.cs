using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public partial class HelpViewModel : ObservableObject, IViewModel
{
    [RelayCommand]
    private void Close(ViewEntityBase view) => CloseAsync(view).Forget();
    private async UniTask CloseAsync(ViewEntityBase view)
    {
        await WeakReferenceMessenger.Default.SendViewHideAsync(view);
    }
}
