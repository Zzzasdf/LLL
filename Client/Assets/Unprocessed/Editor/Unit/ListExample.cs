using System;
using System.Collections.Generic;

public class ListExample
{
    void Foo()
    {
        List<int> a = new List<int>();
        int[] b = new int[5] { 1, 2, 3, 4, 5 };
        string[] c = new string[5] { "1", "2", "3", "4", "5" };
        a.Clear();
        Array.Clear(b, 1, 3);
        Array.Clear(c, 1, 3);
        
        // List 的 Clear 实际上执行的是 Array.Clear
        // Array.Clear 的操作是 将指定一段位置的 item 设置成默认值
        // b 的输出是 { 1, 0, 0, 0, 5 }，因为 item 是值类型，直接在数组中占内存，所以不会产生发生
        // c 的输出是 { "1", null, null, null, "5" } 因为 item 是引用类型，当 引用 被置为 null 时，数据内存不被 引用，产生 GC
        
        // 回收池复用：
        // List<结构体>：只需要回收 List 就行, 因为 结构体数据 直接在数组内存中
        // List<类>：需要回收 List 和 Item, 因为 在触发 Clear 操作后，item 类的引用丢失了，不回收 item 就会产生 GC
    }
}
