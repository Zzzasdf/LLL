using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public partial class CreateRoleViewModel : ObservableRecipient,
    IViewModel
{
    private AccountModel accountModel;

    private string roleName;
    
    public CreateRoleViewModel(AccountModel accountModel)
    {
        this.accountModel = accountModel;
    }
    
    [RelayCommand]
    private void SetRoleName(string roleName)
    {
        this.roleName = roleName;
    }

    [RelayCommand]
    private void Cancel(IView view) => CancelAsync(view).Forget();
    private async UniTask CancelAsync(IView view)
    {
        await WeakReferenceMessenger.Default.SendViewHideAsync(view);
    }

    [RelayCommand]
    private void Confirm() => ConfirmAsync().Forget();
    private async UniTask ConfirmAsync()
    {
        await WeakReferenceMessenger.Default.SendViewConfirmAgainShowAsync("Are you sure?", async () =>
        {
            var accountRoleSimpleModel = accountModel.GetSelectedAccountRoleSimpleModel();
            accountRoleSimpleModel.Id = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            accountRoleSimpleModel.Name = roleName;
            accountRoleSimpleModel.Level = 1;
            await WeakReferenceMessenger.Default.SendDataSaveAccountLevelAsync(accountModel);
            WeakReferenceMessenger.Default.SendProcedureSwap(ProcedureService.GameState.Init);
        });
    }
}
