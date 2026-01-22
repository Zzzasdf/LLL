using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public class ProcedureMain : IProcedure
{
    async UniTask IProcedure.Run()
    {
        await WeakReferenceMessenger.Default.SendViewAllHideAsync();
        LLogger.FrameWarning("进入 Main 场景");
        await WeakReferenceMessenger.Default.SendViewShowAsync<MainView>();
    }
}
