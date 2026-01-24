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
    public static async UniTask<TView> SendViewShowAsync<TView>(this IMessenger messenger) where TView : class, IView
    {
        return await messenger.Send(new ViewShowAsyncRequestEvent(typeof(TView))) as TView;
    }
    public static async UniTask<bool> SendViewHideAsync(this IMessenger messenger, IView view)
    {
        return await messenger.Send(new ViewHideAsyncRequestEvent(view));
    }
    public static UniTask<bool> SendViewAllHideAsync(this IMessenger messenger)
    {
        return UniTask.FromResult(true);
        // return await messenger.Send(new ViewAllHideAsyncRequestEvent());
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
