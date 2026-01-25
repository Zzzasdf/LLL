using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ILayerContainer
{
    ILayerLocator AddLocator(GameObject goLocator);
    
    UniTask<(IView view, int? removeId)> ShowViewAndTryRemoveAsync(Type type);
    UniTask<int?> PopViewAndTryRemove(int uniqueId);

    int? HideViewTryPop(int uniqueId);
    void HideAllView();
    
    void Stash(int uniqueId);
    bool TryStashPop(int uniqueId, out Queue<int> popIds);

    ViewLayer GetViewLayer();
    IViewLoader GetViewLoader();
    ILayerLocator GetLocator();
    void AddViewLocator(GameObject goView);
}
