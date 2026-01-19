using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

[TestFixture]
public class PatchVersionExample : UnityEditor.Editor
{
    [Test]
    public void Foo()
    {
        IntPtr a = new IntPtr(1);
        Debug.LogError($"{a}");
    }
    }
