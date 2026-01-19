using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class RangeExample
{
    [Test]
    public void Foo()
    {
        int nMax = int.MaxValue;
        Debug.Log(nMax + 1);
        int nMin = int.MinValue;
        Debug.Log(nMin - 1);
        
        uint unMax = uint.MaxValue;
        Debug.Log(unMax + 1);
        uint unMin = uint.MinValue;
        Debug.Log(unMin - 1);
    }

    [Test]
    public void Foo2()
    {
        Debug.Log(0xA5);
    }

    [Test]
    public void Foo3()
    {
        const char c = 'm';
        int i = c;
        Debug.Log(i);
        char[] q = new char[1];
        sepctrum sepctrum = (int)sepctrum.blue + sepctrum.red;
        IntPtr d = new IntPtr(5);

        IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(1024);
        Memory<byte> memory = memoryOwner.Memory;
        Span<byte> span = memory.Span;
        BinaryPrimitives.WriteInt16BigEndian(span, 123);
        List<A> a = new List<A>();
        A[] b = new A[77];
        System.Array.Sort(b, (x, y) => -1);
        a.Sort((x, y) => -1);
    }

    public class A
    {
        private int a;
        
        public void AA(in int x)
        {
        }
    }
    
    [StructLayout(LayoutKind.Explicit)]
    public struct DataUnion
    {
        // 所有字段从位置0开始（内存重叠）
        [FieldOffset(0)] public int Integer;
        [FieldOffset(0)] public float Float;
        [FieldOffset(0)] public byte Byte0;
        [FieldOffset(1)] public byte Byte1;
        [FieldOffset(2)] public byte Byte2;
        [FieldOffset(3)] public byte Byte3;
    }
    
    enum sepctrum
    {
        red = 2,
        blue = 4,
    };
}
