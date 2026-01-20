using CommunityToolkit.Mvvm.DependencyInjection;
using Cysharp.Threading.Tasks;

public class ProcedureStart : IProcedure
{
    async UniTask IProcedure.Run()
    {
        await Ioc.Default.ShowViewAsync<StartView>();
    }
}
