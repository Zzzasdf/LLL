using System;
using NUnit.Framework;
using UnityEditor;

[TestFixture]
public class EnumFlagsExample : UnityEditor.Editor
{
    // 1.5倍
    [Test]
    public void Foo()
    {
        int count = 1000000000;
        EAA eaa = EAA.A | EAA.B;
        int ieaa = (int)eaa;
        int d = (int)EAA.A;
        for (int i = 0; i < count; i++)
        {
            bool b = eaa.HasFlag(EAA.A);
        }
    }

    [Flags]
    private enum EAA
    {
        A = 1 << 0,
        B = 1 << 1,
    }
}
