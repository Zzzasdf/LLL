using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public class ProcedureSelectRole : IProcedure
{
    async UniTask IProcedure.Run()
    {
        WeakReferenceMessenger.Default.SendViewAllHideAsync();
        await WeakReferenceMessenger.Default.SendViewShowAsync<SelectRoleView>();
    }
}
