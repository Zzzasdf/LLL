using System;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public static class MessengerExtensions
{
#region Procedure
    public static void SendProcedureSwap(this IMessenger messenger, ProcedureService.GameState gameState)
    {
        messenger.Send(new GameStateMessage(gameState));
    }
#endregion
    
#region Window
    public static async UniTask<bool> SendViewShowAsync<TView>(this IMessenger messenger) where TView : class, IView
    {
        return await messenger.Send(new ViewShowAsyncRequestEvent(typeof(TView)));
    }
    public static async UniTask<bool> SendViewHideAsync(this IMessenger messenger, IView view)
    {
        return await messenger.Send(new ViewHideAsyncRequestEvent(view));
    }
    public static void SendViewAllHideAsync(this IMessenger messenger)
    {
        // messenger.Send(new ViewAllHideAsyncRequestEvent());
    }

    public static async UniTask<bool> SendViewConfirmAgainShowAsync(this IMessenger messenger, string content, Func<UniTask> confirmFunc)
    {
        bool result = await messenger.SendViewShowAsync<ConfirmAgainView>();
        if (!result) return false;
        messenger.Send(new EventDefine.ConfirmAgainViewEvent(content, confirmFunc));
        return true;
    }
#endregion

#region Data
    public static async UniTask<bool> SendDataSaveAccountLevelAsync<T>(this IMessenger messenger, T model) where T: IAccountLevelModel
    {
        DataSaveAccountLevelAsyncRequestEvent d = new DataSaveAccountLevelAsyncRequestEvent(typeof(T), model);
        return await messenger.Send(d);
    }
    public static async UniTask<bool> SendDataSaveRoleLevelAsync<T>(this IMessenger messenger, T model)
    {
        return await messenger.Send(new DataSaveRoleLevelAsyncRequestEvent(typeof(T), model));
    }
    public static async UniTask<bool> SendDataClearRoleLevelAsync(this IMessenger messenger)
    {
        return await messenger.Send(new DataClearRoleLevelAsyncRequestEvent());
    }
#endregion
}
