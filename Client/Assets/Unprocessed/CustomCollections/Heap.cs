using System;
using System.Collections.Generic;
 
/// 堆
public class Heap<T>
{
    private Comparison<T> comparison;
    // 比较符号
    private int compareSymbol;
    protected List<T> heap;

    public Heap(int capacity, Comparison<T> comparison, bool reversalCompare = false)
    {
        this.comparison = comparison;
        compareSymbol = reversalCompare ? -1 : 1;
        heap = new List<T>(capacity);
    }
    public Heap(Comparison<T> comparison, bool reversalCompare = false)
    {
        this.comparison = comparison;
        compareSymbol = reversalCompare ? -1 : 1;
        heap = new List<T>();
    }
    public void Clear() => heap.Clear();
 
    public int Count => heap.Count;
    public bool IsEmpty => heap.Count == 0;
 
    public void Push(T item)
    {
        heap.Add(item);
        HeapifyUp(heap.Count - 1);
    }
 
    public T Peek()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Heap is empty");
        return heap[0];
    }
 
    public T Pop()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Heap is empty");
        T pop = heap[0];
        int lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);
        if (heap.Count > 0)
            HeapifyDown(0);
        return pop;
    }
    
    public void Remove(T t)
    {
        heap.Remove(t);
    }
    
    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (comparison.Invoke(heap[index], heap[parentIndex]) * compareSymbol >= 0)
                break;
            Swap(index, parentIndex);
            index = parentIndex;
        }
    }
 
    private void HeapifyDown(int index)
    {
        while (true)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;
            int smallestIndex = index;
 
            if (leftChildIndex < heap.Count && comparison.Invoke(heap[leftChildIndex], heap[smallestIndex]) * compareSymbol < 0)
                smallestIndex = leftChildIndex;
            
            if (rightChildIndex < heap.Count && comparison.Invoke(heap[rightChildIndex], heap[smallestIndex]) * compareSymbol < 0)
                smallestIndex = rightChildIndex;
            
            if (smallestIndex == index)
                break;
            
            Swap(index, smallestIndex);
            index = smallestIndex;
        }
    }
 
    private void Swap(int i, int j)
    {
        (heap[i], heap[j]) = (heap[j], heap[i]);
    }
 
    // 可选：批量建堆方法（更高效）
    public void BuildHeap(IEnumerable<T> collection)
    {
        heap.Clear();
        heap.AddRange(collection);
        
        for (int i = (heap.Count / 2) - 1; i >= 0; i--)
        {
            HeapifyDown(i);
        }
    }
}