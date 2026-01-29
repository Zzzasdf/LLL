using System.Collections.Generic;

public interface IViewService
{
    Dictionary<ViewLayer, ILayerContainer> GetLayerContainers();
    Dictionary<SubViewDisplay, ISubViewCollectContainer> GetSubViewContainers();
}
