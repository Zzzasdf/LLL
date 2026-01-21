using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public class ProcedureInit : IProcedure
{
    private ILogService logService;
    private IDataService dataService;
    
    public ProcedureInit(IDataService dataService, ILogService logService)
    {
        this.dataService = dataService;
        this.logService = logService;
    }
    async UniTask IProcedure.Run()
    {
        // 初始化组件
        await WeakReferenceMessenger.Default.SendDataClearRoleLevelAsync();
        
        logService.Debug("初始化组件 Test Start.. ");
        RoleModel roleModel = dataService.Get<RoleModel>()
            .Bind(dataService.AccountLevelGet<AccountModel>().GetSelectedAccountRoleSimpleModel());
        roleModel.Test = 111;
        await WeakReferenceMessenger.Default.SendDataSaveRoleLevelAsync(roleModel);
        logService.Debug("初始化组件 Test End.. ");
        
        WeakReferenceMessenger.Default.SendProcedureSwap(ProcedureService.GameState.Main);
    }
}
