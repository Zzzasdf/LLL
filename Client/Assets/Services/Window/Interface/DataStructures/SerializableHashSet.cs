using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class SerializableHashSet<T> : ISerializationCallbackReceiver
{
    [NonSerialized]
    private HashSet<T> hashSet = new HashSet<T>();
    
    [SerializeField]
    private List<T> list = new List<T>();
    
    // HashSet 的主要操作
    public bool Add(T item)
    {
        if (hashSet.Add(item))
        {
            list.Add(item);
            return true;
        }
        return false;
    }
    
    public bool Remove(T item)
    {
        if (hashSet.Remove(item))
        {
            list.Remove(item);
            return true;
        }
        return false;
    }
    
    public void Clear()
    {
        hashSet.Clear();
        list.Clear();
    }
    
    public bool Contains(T item) => hashSet.Contains(item);
    
    // 实现 ISerializationCallbackReceiver
    public void OnBeforeSerialize()
    {
        // 保持 list 与 hashSet 同步
        // 这里我们信任 list 已经是最新的
    }
    
    public void OnAfterDeserialize()
    {
        hashSet.Clear();
        foreach (var item in list)
        {
            // 去重处理
            if (!hashSet.Contains(item))
            {
                hashSet.Add(item);
            }
        }
        
        // 更新 list 以去重
        list = hashSet.ToList();
    }
    
    // 属性
    public int Count => hashSet.Count;
    public bool IsEmpty => hashSet.Count == 0;
    
    // 转换为真正的 HashSet
    public HashSet<T> ToHashSet() => new HashSet<T>(hashSet);
    
    // 从真正的 HashSet 加载
    public void FromHashSet(HashSet<T> source)
    {
        hashSet = new HashSet<T>(source);
        list = hashSet.ToList();
    }
    
    // 迭代器支持
    public IEnumerator<T> GetEnumerator() => hashSet.GetEnumerator();
    
    // 为了方便使用的扩展方法
    public T[] ToArray() => hashSet.ToArray();
    public List<T> ToList() => new List<T>(hashSet);
    
    // 集合操作
    public void UnionWith(IEnumerable<T> other)
    {
        hashSet.UnionWith(other);
        list = hashSet.ToList();
    }
    
    public void IntersectWith(IEnumerable<T> other)
    {
        hashSet.IntersectWith(other);
        list = hashSet.ToList();
    }
    
    public void ExceptWith(IEnumerable<T> other)
    {
        hashSet.ExceptWith(other);
        list = hashSet.ToList();
    }
    
    public bool IsSubsetOf(IEnumerable<T> other) => hashSet.IsSubsetOf(other);
    public bool IsSupersetOf(IEnumerable<T> other) => hashSet.IsSupersetOf(other);
    public bool Overlaps(IEnumerable<T> other) => hashSet.Overlaps(other);
}

// 特定类型的封装（解决Unity泛型序列化限制）
[Serializable]
public class IntHashSet : SerializableHashSet<int> { }

[Serializable]
public class StringHashSet : SerializableHashSet<string> { }

[Serializable]
public class GameObjectHashSet : SerializableHashSet<UnityEngine.GameObject> { }