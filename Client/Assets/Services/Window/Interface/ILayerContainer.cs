using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface ILayerContainer
{
    void BindLocator(ILayerLocator layerLocator);
    UniTask<IView> ShowViewAsync(Type type);
    void HideView(int uniqueId);
    void HideAllView();
    bool TryPopView(int uniqueId);

    bool PushAndTryPop(int uniqueId, out int popId);
    bool PopAndTryPush(int uniqueId, out int pushId);
    
    void StashPush(int uniqueId);
    bool TryStashPop(int uniqueId, out Queue<int> popIds);
}
