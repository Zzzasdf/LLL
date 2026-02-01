using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class SubViewSelectItemLocatorBase : MonoBehaviour
{
    public abstract void Init(ISubViewConfigure subViewConfigure, Func<int, UniTask> selectIndexFunc, int index);
}
