using System.Collections.Generic;

public interface IViewService
{
    void BindLocator(IUICanvasLocator uiCanvasLocator);
    Dictionary<ViewLayer, ILayerContainer> GetLayerContainers();
}
