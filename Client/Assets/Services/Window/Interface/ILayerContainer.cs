using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface ILayerContainer
{
    void BindLocator(ILayerLocator layerLocator);
    
    UniTask<(IView view, int? removeId)> ShowViewAndTryRemoveAsync(Type type);
    int? PopViewAndTryRemove(int uniqueId);

    int? HideViewTryPop(int uniqueId);
    void HideAllView();
    
    void Stash(int uniqueId);
    bool TryStashPop(int uniqueId, out Queue<int> popIds);
}
