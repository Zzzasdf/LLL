using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface ILayerContainer
{
    void Bind(ILayerLocator layerLocator);
    
    UniTask<(IView view, int? removeId)> ShowViewAndTryRemoveAsync(Type type);
    UniTask<List<int>> PopViewAndTryRemove(List<int> popIds);

    List<int> HideViewTryPop(int uniqueId);
    /// 隐藏所有激活 + 暂存界面
    void HideAllView();
    /// 隐藏所有激活界面
    void HideAllActivateView();
    /// 隐藏所有暂存界面
    void HideAllStashView();
    
    void Stash(int uniqueId);
    bool TryStashPop(int uniqueId, out List<int> popIds);
    void StashClear(int uniqueId);

    string ToString();
}
