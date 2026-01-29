using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public partial class EntryButtonGroupViewModel : ObservableObject, IViewModel
{
    [RelayCommand]
    private void ShowActivity() => ShowActivityAsync().Forget();
    private async UniTask ShowActivityAsync()
    {
        await WeakReferenceMessenger.Default.SendViewShowAsync<ActivityView>();
    }
}
