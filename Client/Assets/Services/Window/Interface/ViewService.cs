using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class ViewService: ObservableRecipient, IViewService
{
    private ILayerConfigures layerConfigures;
    private ISubViewCollectConfigures subViewCollectConfigures;

    private Dictionary<ViewLayer, ILayerLocator> layerLocators;
    private Dictionary<SubViewCollect, ISubViewCollectLocator> subViewCollectLocators;
    
    private IViewConfigures viewConfigures;
    private Dictionary<Type, IViewConfigure> viewShows;
    private Dictionary<SubViewShow, IViewConfigure> subViewShows;
    
    public ViewService(ILayerConfigures layerConfigures, ISubViewCollectConfigures subViewCollectConfigures, IViewConfigures viewConfigures)
    {
        this.layerConfigures = layerConfigures;
        this.subViewCollectConfigures = subViewCollectConfigures;
        
        this.viewConfigures = viewConfigures;
        viewShows = new Dictionary<Type, IViewConfigure>();
        subViewShows = new Dictionary<SubViewShow, IViewConfigure>();
        foreach (var pair in viewConfigures)
        {
            ViewLayer viewLayer = pair.Key;
            List<IViewConfigure> configures = pair.Value;
            for (int i = 0; i < configures.Count; i++)
            {
                IViewConfigure viewConfigure = configures[i];
                viewConfigure.AddLayer(viewLayer);
                Type viewType = viewConfigure.GetViewType();
                viewShows.Add(viewType, viewConfigure);
                if (!viewConfigure.TryGetSubViewConfigures(out List<ISubViewConfigure> subViewConfigures))
                {
                    continue;
                }
                for (int j = 0; j < subViewConfigures.Count; j++)
                {
                    ISubViewConfigure subViewConfigure = subViewConfigures[j];
                    if (!subViewConfigure.TryGetSubViewShow(out SubViewShow subViewShow))
                    {
                        continue;
                    }
                    subViewShows.Add(subViewShow, viewConfigure);
                }
            }
        }
        MainViewCheckGenerator.Default.Assembly(viewConfigures);
        IsActive = true;
    }
    
    ILayerConfigures IViewService.GetLayerConfigures() => layerConfigures;
    void IViewService.SetLayerLocators(Dictionary<ViewLayer, ILayerLocator> layerLocators) => this.layerLocators = layerLocators;

    ISubViewCollectConfigures IViewService.GetSubViewCollectConfigures() => subViewCollectConfigures;
    void IViewService.SetSubViewCollectLocators(Dictionary<SubViewCollect, ISubViewCollectLocator> subViewCollectLocators) => this.subViewCollectLocators = subViewCollectLocators;
}