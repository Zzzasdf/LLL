using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public class ProcedureSelectRole : IProcedure
{
    async UniTask IProcedure.Run()
    {
        await WeakReferenceMessenger.Default.SendViewAllHideAsync();
        await WeakReferenceMessenger.Default.SendViewShowAsync<SelectRoleView>();
    }
}
