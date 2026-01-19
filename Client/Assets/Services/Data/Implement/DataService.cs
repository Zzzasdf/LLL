using System.IO;
using UnityEngine;

public class DataService : IDataService
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
            return Path.Combine(Application.persistentDataPath, $"role_{accountLevelCache.Get<AccountModel>().SelectedIndex()}");
        });
    }

    T IDataService.AccountLevelGet<T>() => accountLevelCache.Get<T>();

    void IDataService.AccountLevelSave<T>(T data) => accountLevelCache.Save(data);
    
    T IDataService.Get<T>() => roleLevelCache.Get<T>();

    void IDataService.Save<T>(T data) => roleLevelCache.Save(data);
}
