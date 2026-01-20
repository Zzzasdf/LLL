using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public class ProcedurePreload : IProcedure
{
    UniTask IProcedure.Run()
    {
        IViewService viewService = Ioc.Default.GetService<IViewService>();
        
        // 预编译 Shader
        Ioc.Default.GetService<ILogService>().Warning("预编译 Shader..");
        
        
        WeakReferenceMessenger.Default.Send(new GameStateMessage
        {
            gameState = ProcedureService.GameState.Start,
        });
        return UniTask.CompletedTask;
    }
}
