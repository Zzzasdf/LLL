using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface IViewLayerLocator
{
    void Init(ViewLayer viewLayer, List<IViewConfigure> viewConfigures);
    List<IViewConfigure> GetViewConfigures();
    
    IViewLayerContainer GetContainer();

    UniTask<IView> ShowViewAsync(Type type);
    UniTask<bool> TryPopViewAsync(List<int> uniqueIds);

    void HideView(int uniqueId);
    void PushHideView(int uniqueId);
}
