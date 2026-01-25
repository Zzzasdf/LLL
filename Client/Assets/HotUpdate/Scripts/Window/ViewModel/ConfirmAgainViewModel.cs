using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public partial class ConfirmAgainViewModel : ObservableRecipient, IViewModel,
    IRecipient<EventDefine.ConfirmAgainViewEvent>
{
    [ObservableProperty]
    private EventDefine.ConfirmAgainViewEvent _confirmAgainViewEvent;
    
    [RelayCommand]
    private void Cancel(IView view) => CancelAsync(view).Forget();
    private async UniTask CancelAsync(IView view)
    {
        await WeakReferenceMessenger.Default.SendViewHideAsync(view);
    }

    [RelayCommand]
    private void Confirm(IView view) => ConfirmAsync(view).Forget();
    private async UniTask ConfirmAsync(IView view)
    {
        await CancelAsync(view);
        await ConfirmAgainViewEvent.ConfirmFunc.Invoke();
    }

    void IRecipient<EventDefine.ConfirmAgainViewEvent>.Receive(EventDefine.ConfirmAgainViewEvent message)
    {
        ConfirmAgainViewEvent = message;
    }
}
