using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface IViewService
{
    void BindLocator(UICanvasLocator uiCanvasLocator);
    Dictionary<ViewLayer, ILayerContainer> GetLayerContainers();
    
    UniTask<T> ShowAsync<T>(ViewLayer viewLayer) where T: class, IView;
    UniTask<bool> HideAsync(IView view);
}
