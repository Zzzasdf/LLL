using System.Collections;
using System.Collections.Generic;

public class LayerConfigures: ILayerConfigures
{
    private Dictionary<ViewLayer, ILayerConfigure> layerConfigures;
    
    public LayerConfigures(Dictionary<ViewLayer, ILayerConfigure> layerConfigures)
    {
        this.layerConfigures = layerConfigures;
    }

    IEnumerator<KeyValuePair<ViewLayer, ILayerConfigure>> IEnumerable<KeyValuePair<ViewLayer, ILayerConfigure>>.GetEnumerator()
    {
        return layerConfigures.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return layerConfigures.GetEnumerator();
    }
}
