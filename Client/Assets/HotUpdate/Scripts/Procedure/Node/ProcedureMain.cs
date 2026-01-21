using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProcedureMain : IProcedure
{
    async UniTask IProcedure.Run()
    {
        await WeakReferenceMessenger.Default.SendViewAllHideAsync();
        Debug.LogError("进入 Main 场景");
    }
}
