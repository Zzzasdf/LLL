using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface ILayerLocator
{
    void Build(ILayerContainer layerContainer, IUICanvasLocator uiCanvasLocator);
    ILayerContainer GetContainer();
    IUICanvasLocator GetCanvasLocator();

    UniTask<IView> ShowViewAsync(Type type);
    UniTask<bool> TryPopViewAsync(List<int> uniqueIds);

    void HideView(int uniqueId);
    void PushHideView(int uniqueId);
}
