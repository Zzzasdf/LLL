using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ILayerContainer
{
    ILayerLocator AddLocator(GameObject goLocator);
    
    UniTask<(IView view, int? removeId)> ShowViewAndTryRemoveAsync(Type type);
    UniTask<int?> PopViewAndTryRemove(int uniqueId, int siblingIndex);
    UniTask<int?> PopViewAndTryRemove(Queue<int> uniqueIds);

    (int? popId, int siblingIndex) HideViewTryPop(int uniqueId);
    /// 隐藏所有激活 + 暂存界面
    void HideAllView();
    /// 隐藏所有激活界面
    void HideAllActivateView();
    /// 隐藏所有暂存界面
    void HideAllStashView();
    
    void Stash(int uniqueId);
    bool TryStashPop(int uniqueId, out Queue<int> popIds);

    ViewLayer GetViewLayer();
    IViewLoader GetViewLoader();
    ILayerLocator GetLocator();
    void AddViewLocator(GameObject goView);

    string ToString();
}
