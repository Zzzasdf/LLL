using System;
using Cysharp.Threading.Tasks;

public interface ILayerLocator
{
    void Build(ILayerContainer layerContainer, IUICanvasLocator uiCanvasLocator);
    ILayerContainer GetContainer();
    IUICanvasLocator GetCanvasLocator();

    Type GetViewType(int uniqueId);
    IView GetView(int uniqueId);
    bool ExistInstantiate(int uniqueId);
    
    UniTask<IView> ShowViewAsync(Type type);
    UniTask<bool> TryPopViewAsync(int uniqueId, int siblingIndex);
    void HideView(int uniqueId);
    void PushHideView(int uniqueId);
}
