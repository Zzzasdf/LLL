using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableStack<T> : IEnumerable<T>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<T> list = new List<T>();
    
    private Stack<T> stack;
    private bool useStackInternally = true; // 控制是否使用内部Stack缓存
    
    // 构造函数
    public SerializableStack()
    {
        stack = new Stack<T>();
    }
    
    public SerializableStack(IEnumerable<T> collection)
    {
        stack = new Stack<T>(collection);
        SyncToSerializedList();
    }
    
    public SerializableStack(int capacity)
    {
        stack = new Stack<T>(capacity);
    }
    
    // Stack 的主要操作
    public void Push(T item)
    {
        EnsureStack();
        stack.Push(item);
        if (useStackInternally)
        {
            SyncToSerializedList();
        }
    }
    
    public T Pop()
    {
        EnsureStack();
        T item = stack.Pop();
        if (useStackInternally)
        {
            SyncToSerializedList();
        }
        return item;
    }
    
    public T Peek()
    {
        EnsureStack();
        return stack.Peek();
    }
    
    public void Clear()
    {
        EnsureStack();
        stack.Clear();
        if (useStackInternally)
        {
            SyncToSerializedList();
        }
    }
    
    public bool Contains(T item)
    {
        EnsureStack();
        return stack.Contains(item);
    }
    
    public T[] ToArray()
    {
        EnsureStack();
        return stack.ToArray();
    }
    
    public void TrimExcess()
    {
        EnsureStack();
        stack.TrimExcess();
        if (useStackInternally)
        {
            SyncToSerializedList();
        }
    }
    
    // 属性
    public int Count
    {
        get
        {
            EnsureStack();
            return stack.Count;
        }
    }
    
    public bool IsEmpty => Count == 0;
    
    // 实现 IEnumerable<T>
    public IEnumerator<T> GetEnumerator()
    {
        EnsureStack();
        return stack.GetEnumerator();
    }
    
    // 实现 IEnumerable (非泛型版本)
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    // 实现 ISerializationCallbackReceiver
    public void OnBeforeSerialize()
    {
        if (useStackInternally)
        {
            SyncToSerializedList();
        }
    }
    
    public void OnAfterDeserialize()
    {
        if (useStackInternally)
        {
            EnsureStackFromList();
        }
    }
    
    // 辅助方法
    private void EnsureStack()
    {
        if (stack == null)
        {
            stack = new Stack<T>();
        }
    }
    
    private void EnsureStackFromList()
    {
        stack = new Stack<T>();
        // 注意：列表中的顺序是栈底到栈顶
        // 所以我们需要反向添加到Stack中
        for (int i = list.Count - 1; i >= 0; i--)
        {
            stack.Push(list[i]);
        }
    }
    
    private void SyncToSerializedList()
    {
        list.Clear();
        
        // Stack.ToArray() 返回的顺序是栈顶到栈底
        T[] array = stack.ToArray();
        
        // 我们希望在Inspector中显示栈底在顶部，栈顶在底部
        // 所以需要反转数组
        for (int i = array.Length - 1; i >= 0; i--)
        {
            list.Add(array[i]);
        }
    }
    
    // 从列表重建Stack（手动调用）
    public void RebuildStackFromList()
    {
        EnsureStackFromList();
    }
    
    // 更新列表以匹配Stack（手动调用）
    public void UpdateListFromStack()
    {
        SyncToSerializedList();
    }
    
    // 为了方便调试，添加ToString方法
    public override string ToString()
    {
        EnsureStack();
        return $"SerializableStack<{typeof(T).Name}> Count: {Count}";
    }
    
    // 索引器（只读）- 用于直接访问列表中的元素
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= list.Count)
                throw new IndexOutOfRangeException();
            
            // 注意：list[0] 是栈底，list[Count-1] 是栈顶
            return list[index];
        }
    }
    
    // 转换为只读列表
    public IReadOnlyList<T> AsReadOnlyList()
    {
        return list.AsReadOnly();
    }
    
    // 使用Stack进行批量操作
    public void PushRange(IEnumerable<T> items)
    {
        EnsureStack();
        foreach (T item in items)
        {
            stack.Push(item);
        }
        if (useStackInternally)
        {
            SyncToSerializedList();
        }
    }
    
    // 尝试Pop，避免异常
    public bool TryPop(out T result)
    {
        EnsureStack();
        if (stack.Count > 0)
        {
            result = stack.Pop();
            if (useStackInternally)
            {
                SyncToSerializedList();
            }
            return true;
        }
        
        result = default;
        return false;
    }
    
    // 尝试Peek，避免异常
    public bool TryPeek(out T result)
    {
        EnsureStack();
        if (stack.Count > 0)
        {
            result = stack.Peek();
            return true;
        }
        
        result = default;
        return false;
    }
}