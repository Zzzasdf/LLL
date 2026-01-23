using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public partial class SelectRoleViewModel : ObservableRecipient,
    IViewModel,
    IRecipient<EventDefine.SelectedRoleArchiveEvent>
{
    private readonly AccountModel accountModel;
    
    public SelectRoleViewModel(AccountModel accountModel)
    {
        this.accountModel = accountModel;
    }

    [RelayCommand]
    private void Return()
    {
        WeakReferenceMessenger.Default.SendProcedureSwap(ProcedureService.GameState.Start);
    }

    [RelayCommand]
    private void Close(IView view) => CloseAsync(view).Forget();
    private async UniTask CloseAsync(IView view)
    {
        await WeakReferenceMessenger.Default.SendViewHideAsync(view);
    }
    
    public AccountRoleSimpleModel GetOrAddRoleModel(int index)
    {
        return accountModel.GetOrAddAccountRoleSimpleModel(index);
    }
    
    async void IRecipient<EventDefine.SelectedRoleArchiveEvent>.Receive(EventDefine.SelectedRoleArchiveEvent message)
    {
        accountModel.SetSelectedIndex(message.AccountRoleSimpleModel);
        if (message.AccountRoleSimpleModel.Id == default)
        {
            WeakReferenceMessenger.Default.SendProcedureSwap(ProcedureService.GameState.CreateRole);
        }
        else
        {
            await WeakReferenceMessenger.Default.SendDataSaveAccountLevelAsync(accountModel);
            WeakReferenceMessenger.Default.SendProcedureSwap(ProcedureService.GameState.Init);
        }
    }
}
