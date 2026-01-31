using System.Collections.Generic;

public interface IViewService
{
    ILayerConfigures GetLayerConfigures();
    void SetLayerLocators(Dictionary<ViewLayer, ILayerLocator> layerLocators);

    ISubViewCollectConfigures GetSubViewCollectConfigures();
    void SetSubViewCollectLocators(Dictionary<SubViewCollect, ISubViewCollectLocator> subViewCollectLocators);

    List<IViewConfigure> GetViewConfigures(ViewLayer viewLayer);
}
