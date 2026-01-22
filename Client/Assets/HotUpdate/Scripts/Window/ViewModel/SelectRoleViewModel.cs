using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

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
