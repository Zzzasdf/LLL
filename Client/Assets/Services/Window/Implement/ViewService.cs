using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class ViewService: ObservableRecipient, IViewService
{
    public ViewService(Dictionary<ViewLayer, ILayerContainer> layerContainers, Dictionary<ViewLayer, List<IViewConfigure>> views, 
        Dictionary<SubViewDisplay, ISubViewCollectContainer> subViewContainers, List<ISubViewConfigure> subViews)
    {
        InitViews(layerContainers, views);
        InitSubViews(subViewContainers, subViews);
        IsActive = true;
    }
    
    Dictionary<ViewLayer, ILayerContainer> IViewService.GetLayerContainers() => layerContainers;
    Dictionary<SubViewDisplay, ISubViewCollectContainer> IViewService.GetSubViewContainers() => subViewContainers;
}