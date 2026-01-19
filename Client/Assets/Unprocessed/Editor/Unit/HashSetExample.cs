using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class HashSetExample 
{
    [Test]
    public void Foo()
    {
        HashSet<int> a = new HashSet<int>()
        {
            1,2
        };
        HashSet<int> b = new HashSet<int>()
        {
            2,3
        };
        a.AddRange(b);
        foreach (var _a in a)
        {
            Debug.Log(_a);
        }
    }
}