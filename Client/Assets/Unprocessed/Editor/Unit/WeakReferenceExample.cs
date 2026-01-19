using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class WeakReferenceExample
{
    private static WeakReference<List<int>> a = new WeakReference<List<int>>(null);
    private static List<int> d;
    
    [Test]
    public void Foo()
    {
        d = new List<int>() { 1 };
        List<int> b = new List<int>() { 1 };
        a.SetTarget(b);
        Debug.Log(a.TryGetTarget(out List<int> c));
        CDebug.Log(c);
    }

    [Test]
    public void Foo2()
    {
        Debug.LogError(a.TryGetTarget(out List<int> c));
        Debug.LogError(d.Count);
    }
}
