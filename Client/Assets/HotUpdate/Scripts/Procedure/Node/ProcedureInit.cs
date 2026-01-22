using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public class ProcedureInit : IProcedure
{
    private IDataService dataService;
    
    public ProcedureInit(IDataService dataService)
    {
        this.dataService = dataService;
    }
    async UniTask IProcedure.Run()
    {
        // 初始化组件
        await WeakReferenceMessenger.Default.SendDataClearRoleLevelAsync();
        
        LLogger.Log("初始化组件 Test Start.. ");
        RoleModel roleModel = dataService.Get<RoleModel>()
            .Bind(dataService.AccountLevelGet<AccountModel>().GetSelectedAccountRoleSimpleModel());
        roleModel.Test = 111;
        await WeakReferenceMessenger.Default.SendDataSaveRoleLevelAsync(roleModel);
        LLogger.Log("初始化组件 Test End.. ");
        
        WeakReferenceMessenger.Default.SendProcedureSwap(ProcedureService.GameState.Main);
    }
}
