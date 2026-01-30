using System.Collections;
using System.Collections.Generic;

public class ViewConfigures : IViewConfigures
{
    private Dictionary<ViewLayer, List<IViewConfigure>> viewConfigures;
    
    public ViewConfigures(Dictionary<ViewLayer, List<IViewConfigure>> viewConfigures)
    {
        this.viewConfigures = viewConfigures;
    }

    IEnumerator<KeyValuePair<ViewLayer, List<IViewConfigure>>> IEnumerable<KeyValuePair<ViewLayer, List<IViewConfigure>>>.GetEnumerator()
    {
        return viewConfigures.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return viewConfigures.GetEnumerator();
    }
}
