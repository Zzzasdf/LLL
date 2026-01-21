using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

public class LevelDataService
{
    private Func<string> folderFunc;
    private Dictionary<Type, object> levelCache;

    public LevelDataService(Func<string> folderFunc)
    {
        this.folderFunc = folderFunc;
        levelCache = new Dictionary<Type, object>();
    }

    public void Clear()
    {
        levelCache.Clear();
    }
    
    public T Get<T>() where T: new()
    {
        Type type = typeof(T);
        if (levelCache.TryGetValue(type, out object value))
        {
            return (T)value;
        }
        string folderPath = folderFunc.Invoke();
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string filePath = Path.Combine(folderPath, $"{typeof(T).Name}.json");
        if (!File.Exists(filePath))
        {
            T result = new T();
            levelCache.Add(type, result);
            return result;
        }
        try
        {
            string jsonString = File.ReadAllText(filePath);
            T result = JsonConvert.DeserializeObject<T>(jsonString);
            levelCache.Add(type, result);
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError($"读档 {type.Name} 失败，删除损坏文件，重新生成：{e.Message}");
            File.Delete(filePath);
            T result = new T();
            levelCache.Add(type, result);
            return result;
        }
    }
    
    public async UniTask<bool> SaveAsync(Type type, object data)
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
        try
        {
            string filePath = Path.Combine(folderFunc.Invoke(), $"{type.Name}.json");
            await File.WriteAllTextAsync(filePath, jsonString);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"存档 {type.Name} 失败: {e.Message}");
        }
        return false;
    }
}
