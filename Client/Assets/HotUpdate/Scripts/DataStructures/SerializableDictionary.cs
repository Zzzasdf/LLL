using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TKey, TValue>>
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();
    
    [SerializeField]
    private List<TValue> values = new List<TValue>();
    
    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
    
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var pair in dictionary)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
    
    public void OnAfterDeserialize()
    {
        dictionary.Clear();
        for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
        {
            dictionary[keys[i]] = values[i];
        }
    }
    
    public TValue this[TKey key]
    {
        get { return dictionary[key]; }
        set { dictionary[key] = value; }
    }

    public int Count => dictionary.Count;
    
    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    public void Clear() => dictionary.Clear();
    
    public void Add(TKey key, TValue value) => dictionary.Add(key, value);
    public bool TryAdd(TKey key, TValue value) => dictionary.TryAdd(key, value);
    
    public bool Remove(TKey key) => dictionary.Remove(key);
    public bool Remove(TKey key, out TValue value) => dictionary.Remove(key, out value);
    
    public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);
    
    public Dictionary<TKey,TValue>.KeyCollection Keys => dictionary.Keys;
    
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => dictionary.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();
}