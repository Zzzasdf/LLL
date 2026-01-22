using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public class ProcedurePreload : IProcedure
{
    UniTask IProcedure.Run()
    {
        // 预编译 Shader
        LLogger.Warning("预编译 Shader..");
        
        WeakReferenceMessenger.Default.SendProcedureSwap(ProcedureService.GameState.Start);
        return UniTask.CompletedTask;
    }
}
