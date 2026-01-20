using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Object = System.Object;

public class LevelDataService
{
    private Func<string> folderFunc;
    private Dictionary<Type, Object> levelCache;

    public LevelDataService(Func<string> folderFunc)
    {
        this.folderFunc = folderFunc;
        levelCache = new Dictionary<Type, Object>();
    }
    
    public T Get<T>() where T: new()
    {
        Type type = typeof(T);
        if (levelCache.TryGetValue(type, out Object value))
        {
            return (T)value;
        }
        string filePath = Path.Combine(folderFunc.Invoke(), nameof(T));
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
            Debug.LogError($"读档{nameof(type)}失败，删除损坏文件，重新生成：{e.Message}");
            File.Delete(filePath);
            T result = new T();
            levelCache.Add(type, result);
            return result;
        }
    }
    
    public void Save<T>(T data) where T : new()
    {
        string jsonString = JsonConvert.SerializeObject(data);
        try
        {
            string filePath = Path.Combine(folderFunc.Invoke(), nameof(T));
            File.WriteAllText(filePath, jsonString);
        }
        catch (Exception e)
        {
            Debug.LogError($"存档{typeof(T).Name}失败: {e.Message}");
        }
    }
}
