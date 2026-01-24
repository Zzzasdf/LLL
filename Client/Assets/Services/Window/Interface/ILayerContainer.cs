using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface ILayerContainer
{
    void BindLocator(ILayerLocator layerLocator);
    
    UniTask<IView> ShowViewAsync(Type type);
    void HideView(int uniqueId);
    bool TryPopView(int uniqueId);

    void HideAllView();

    bool PushAndTryPop(int uniqueId, out int popId);
    bool PopAndTryPush(int uniqueId, out int pushId);
    
    void StashPush(int uniqueId);
    bool TryStashPop(int uniqueId, out Queue<int> popIds);
}
