using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DataService : ObservableRecipient, 
    IDataService,
    IRecipient<DataSaveAccountLevelAsyncRequestEvent>, 
    IRecipient<DataSaveRoleLevelAsyncRequestEvent>,
    IRecipient<DataClearRoleLevelAsyncRequestEvent>
{
    private LevelDataService accountLevelCache;
    private LevelDataService roleLevelCache;
    
    public DataService()
    {
        accountLevelCache = new LevelDataService(() =>
        {
            return Application.persistentDataPath;
        });
        roleLevelCache = new LevelDataService(() =>
        {
            return Path.Combine(Application.persistentDataPath, $"role_{accountLevelCache.Get<AccountModel>().GetSelectedIndex()}");
        });
        IsActive = true;
    }

    T IDataService.AccountLevelGet<T>() => accountLevelCache.Get<T>();
    T IDataService.Get<T>() => roleLevelCache.Get<T>();
    
    void IRecipient<DataSaveAccountLevelAsyncRequestEvent>.Receive(DataSaveAccountLevelAsyncRequestEvent message)
    {
        message.Reply(accountLevelCache.SaveAsync(message.Type, message.Model).AsTask());
    }

    void IRecipient<DataSaveRoleLevelAsyncRequestEvent>.Receive(DataSaveRoleLevelAsyncRequestEvent message)
    {
        message.Reply(roleLevelCache.SaveAsync(message.Type, message.Model).AsTask());
    }
    void IRecipient<DataClearRoleLevelAsyncRequestEvent>.Receive(DataClearRoleLevelAsyncRequestEvent message)
    {
        roleLevelCache.Clear();
        message.Reply(true);
    }
}
