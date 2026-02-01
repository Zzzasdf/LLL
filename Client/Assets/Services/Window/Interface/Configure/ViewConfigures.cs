using System.Collections;
using System.Collections.Generic;

public class ViewConfigures : IViewConfigures
{
    private Dictionary<ViewLayer, IViewConfigure> configures;

    public ViewConfigures(Dictionary<ViewLayer, IViewConfigure> configures)
    {
        this.configures = configures;
    }

    IEnumerator<KeyValuePair<ViewLayer, IViewConfigure>> IEnumerable<KeyValuePair<ViewLayer, IViewConfigure>>.GetEnumerator() => configures.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => configures.GetEnumerator();
}
