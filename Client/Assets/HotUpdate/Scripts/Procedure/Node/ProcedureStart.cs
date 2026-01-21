using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public class ProcedureStart : IProcedure
{
    async UniTask IProcedure.Run()
    {
        await WeakReferenceMessenger.Default.SendViewAllHideAsync();
        await WeakReferenceMessenger.Default.SendViewShowAsync<StartView>();
    }
}
