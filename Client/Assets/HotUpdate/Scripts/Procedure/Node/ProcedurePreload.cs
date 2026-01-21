using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public class ProcedurePreload : IProcedure
{
    private ILogService logService;
    
    public ProcedurePreload(ILogService logService)
    {
        this.logService = logService;
    }
    
    UniTask IProcedure.Run()
    {
        // 预编译 Shader
        logService.Warning("预编译 Shader..");
        
        WeakReferenceMessenger.Default.SendProcedureSwap(ProcedureService.GameState.Start);
        return UniTask.CompletedTask;
    }
}
