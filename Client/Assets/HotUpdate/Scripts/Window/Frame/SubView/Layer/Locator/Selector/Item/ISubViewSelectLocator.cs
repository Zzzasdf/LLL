using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISubViewSelectLocator
{
    void Init(List<ISubViewConfigure> subViewConfigures, Func<RectTransform, int, UniTask> selectIndexFunc, int firstShowIndex);
}
