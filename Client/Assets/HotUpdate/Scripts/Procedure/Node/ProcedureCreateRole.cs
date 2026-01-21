using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public class ProcedureCreateRole : IProcedure
{
    async UniTask IProcedure.Run()
    {
        await WeakReferenceMessenger.Default.SendViewAllHideAsync();
        await WeakReferenceMessenger.Default.SendViewShowAsync<CreateRoleView>();
    }
}
